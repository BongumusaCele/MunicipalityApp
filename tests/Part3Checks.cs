using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using MunicipalityApp.Structures;
using MunicipalityApp.Models;
using MunicipalityApp.Data;
using MunicipalityApp.Forms;
class Part3Checks
{
    static int checks;
    static void Check(bool condition, string name) { checks++; if (!condition) throw new Exception(name); }
    static int Avl(BinaryNode<int,int> n) { if(n==null)return 0; int a=Avl(n.Left),b=Avl(n.Right); Check(Math.Abs(a-b)<=1 && n.Height==1+Math.Max(a,b),"AVL balance/height");return 1+Math.Max(a,b); }
    static int RedBlack(BinaryNode<int,int> n) { if(n==null)return 1; Check(n.Right==null || !n.Right.Red,"left leaning"); Check(!n.Red || ((n.Left==null || !n.Left.Red) && (n.Right==null || !n.Right.Red)),"no consecutive red");int a=RedBlack(n.Left),b=RedBlack(n.Right);Check(a==b,"equal black height");return a+(n.Red?0:1); }
    [STAThread] static void Main()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("MIDVAAL_DATA_DIRECTORY"))) throw new Exception("Set MIDVAAL_DATA_DIRECTORY to an isolated test folder first.");
        var random=new Random(42);
        foreach(var keys in new[]{Enumerable.Range(0,500).ToArray(),Enumerable.Range(0,500).Reverse().ToArray(),Enumerable.Range(0,500).OrderBy(x=>random.Next()).ToArray()})
        {
            var avl=new AvlTree<int,int>();var rb=new RedBlackTree<int,int>();var bst=new SearchTree<int,int>();
            foreach(int k in keys){avl.Put(k,k);rb.Put(k,k);bst.Put(k,k);}
            Avl(avl.Root);Check(!rb.Root.Red,"black root");RedBlack(rb.Root);
            foreach(var tree in new SearchTree<int,int>[]{avl,rb,bst}) {Check(tree.Ordered().SequenceEqual(Enumerable.Range(0,500)),"sorted traversal"); tree.Put(10,999);int found;Check(tree.TryGet(10,out found)&&found==999,"replace duplicate");Check(!tree.TryGet(-1,out found),"missing key");}
        }
        var heap=new MinHeap<int>((a,b)=>a.CompareTo(b));var input=Enumerable.Range(0,1000).Select(x=>random.Next(50)).ToArray();foreach(int k in input)heap.Push(k);var output=new List<int>();while(heap.Count>0)output.Add(heap.Pop());Check(output.SequenceEqual(input.OrderBy(x=>x)),"heap order and duplicates");
        bool empty=false;try{heap.Pop();}catch(InvalidOperationException){empty=true;}Check(empty,"empty heap");
        var graph=new RequestGraph();graph.Connect("A","B",5);graph.Connect("A","C",1);graph.Connect("C","B",2);graph.Connect("B","D",1);graph.AddVertex("isolated");graph.Connect("X","Y",2);
        Check(graph.BreadthFirst("A").SequenceEqual(new[]{"A","B","C","D"}),"BFS cycle handling");Check(graph.BreadthFirst("missing").Count==0,"missing vertex");Check(graph.BreadthFirst("isolated").Count==1,"isolated vertex");var forest=graph.MinimumSpanningForest();Check(forest.Count==4&&forest.Sum(e=>e.Weight)==6,"minimum spanning disconnected forest");
        var issues=new List<MunicipalIssue>{new MunicipalIssue{Reference="MDV-1",Category="Roads",Location="A",Status=IssueStatus.Resolved},new MunicipalIssue{Reference="MDV-2",Category="Roads",Location="B",Status=IssueStatus.Open},new MunicipalIssue{Reference="MDV-3",Category="Water",Location="B",Status=IssueStatus.InProgress}};
        var index=new ServiceRequestIndex(issues);MunicipalIssue match;Check(index.ByReference.TryGet("mdv-2",out match),"case insensitive tracking");Check(index.Display("Open","Newest first").Single().Reference=="MDV-2","status BST filter");Check(index.Display("All statuses","Priority (active first)").Select(i=>i.Reference).SequenceEqual(new[]{"MDV-2","MDV-3","MDV-1"}),"priority display");Check(index.Graph.BreadthFirst("MDV-1").Count==3,"indirect relationship");
        Check(new ServiceRequestIndex(new List<MunicipalIssue>()).Display("All statuses","Newest first").Count==0,"empty index");
        var collisionDate = new DateTime(2026, 11, 13);
        foreach (var issue in issues) issue.DateReported = collisionDate;
        var sameDateIndex = new ServiceRequestIndex(issues);
        Check(sameDateIndex.Display("All statuses", "Oldest first").Count == 3, "timestamp collisions retained");
        Check(sameDateIndex.Display("All statuses", "Newest first").Select(i=>i.Reference).SequenceEqual(new[]{"MDV-3","MDV-2","MDV-1"}), "deterministic date tie order");
        Application.EnableVisualStyles();
        using(var form=new ServiceRequestStatusForm()) {form.Show();Application.DoEvents();
            var flags=BindingFlags.Instance|BindingFlags.NonPublic;
            var referenceBox=(TextBox)typeof(ServiceRequestStatusForm).GetField("reference",flags).GetValue(form);
            var requestGrid=(DataGridView)typeof(ServiceRequestStatusForm).GetField("grid",flags).GetValue(form);
            referenceBox.Text="  mdv-1004  ";typeof(ServiceRequestStatusForm).GetMethod("Track",flags).Invoke(form,null);
            Check(requestGrid.Rows.Count==1 && ((MunicipalIssue)requestGrid.Rows[0].DataBoundItem).Reference=="MDV-1004", "form track normalisation");
            referenceBox.Text="unknown";typeof(ServiceRequestStatusForm).GetMethod("Track",flags).Invoke(form,null);
            var summaryLabel=(Label)typeof(ServiceRequestStatusForm).GetField("summary",flags).GetValue(form);
            Check(summaryLabel.Text.Contains("not found"), "form unknown reference message");
            typeof(ServiceRequestStatusForm).GetMethod("Reload",flags).Invoke(form,null);
            Check(requestGrid.Rows.Count>=4, "form refresh restores list");
            using(var bitmap=new Bitmap(form.Width,form.Height)){form.DrawToBitmap(bitmap,new Rectangle(0,0,form.Width,form.Height));bitmap.Save("tests/part3-status.png");}form.Hide();}
        using(var form=new MunicipalityApp.MainMenuForm()){form.Show();Application.DoEvents();form.Hide();}
        Console.WriteLine("PASS: "+checks+" assertions; status and main forms instantiated and rendered.");
    }
}



