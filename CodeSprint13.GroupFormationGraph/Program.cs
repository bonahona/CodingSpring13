using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Solution
{
    class Student
    {
        public String Name { get; set; }
        public int Grade { get; set; }
    }

    class GraphNode<T>
    {
        public T Value { get; set; }
        public HashSet<GraphNode<T>> Children { get; set; }

        public GraphNode()
        {
            Children = new HashSet<GraphNode<T>>();
        }

        public void Connect(GraphNode<T> other) => Children.Add(other);

        public HashSet<GraphNode<T>> AllDecendants()
        {
            var result = new HashSet<GraphNode<T>>();

            GetAllChildren(result, this);
            return result;
        }

        private void GetAllChildren(HashSet<GraphNode<T>> descendants, GraphNode<T> current)
        {
            if (descendants.Contains(current)) {
                return;
            }

            descendants.Add(current);
            foreach(var child in current.Children) {
                GetAllChildren(descendants, child);
            }
        }
    }

    class Constraints
    {
        public int MinSize { get; set; }
        public int MaxSize { get; set; }
        public Dictionary<int, int> GradeSize { get; set; }

        public Constraints(int minSize, int maxSize, int gradeSize01, int gradeSize02, int gradeSize03)
        {
            MinSize = minSize;
            MaxSize = maxSize;
            GradeSize = new Dictionary<int, int>(3) {
                { 1, gradeSize01 },
                { 2, gradeSize02 },
                { 3, gradeSize03 }
            };
        }
    }

    static void membersInTheLargestGroups(int n, int m, int a, int b, int f, int s, int t)
    {
        var constraints = new Constraints(a, b, f, s, t);

        var graphs = ReadStudents(n);
        ReadRequests(m, graphs, constraints);

        var names = FindNamesOfLargestGroups(graphs.Values.ToList(), constraints);

        if(names.Count == 0) {
            Console.WriteLine("no groups");
        }else{
            foreach(var name in names) {
                Console.WriteLine(name);
            }
        }
    }

    static Dictionary<String, GraphNode<Student>> ReadStudents(int n)
    {
        var result = new Dictionary<String, GraphNode<Student>>();
        for (int i = 0; i < n; i++) {
            var student = Console.ReadLine().Split(' ');
            result.Add(student[0], new GraphNode<Student> { Value = new Student { Name = student[0], Grade = Convert.ToInt32(student[1]) } });
        }

        return result;
    }

    static void ReadRequests(int requestCount, Dictionary<String, GraphNode<Student>> students, Constraints constraints)
    {
        for (int i = 0; i < requestCount; i++) {
            var request = Console.ReadLine().Split(' ');
            HandleRequest(students[request[0]], students[request[1]], constraints);
        }
    }

    static void HandleRequest(GraphNode<Student> student01, GraphNode<Student> student02, Constraints constraints)
    {
        var student01Decendants = student01.AllDecendants();
        var student02Decendants = student02.AllDecendants();

        if (student01Decendants.Contains(student02)) {
            return;
        }

        if (student01Decendants.Count + student02Decendants.Count > constraints.MaxSize) {
            return;
        }

        for(int i = 1; i <= 3; i++) {
            if(!GradeFits(i, student01Decendants, student02Decendants, constraints)) {
                return;
            }
        }

        student01.Connect(student02);
        student02.Connect(student01);
    }

    static bool GradeFits(int grade, HashSet<GraphNode<Student>> a, HashSet<GraphNode<Student>> b, Constraints constraints)
    {
        return a.Select(s => s.Value).Where(s => s.Grade == grade).Count() + b.Select(s => s.Value).Where(s => s.Grade == grade).Count() <= constraints.GradeSize[grade];
    }

    static HashSet<GraphNode<Student>> FindDistictGraphs(List<GraphNode<Student>> graphs)
    {
        var result = new HashSet<GraphNode<Student>>();
        var visitedNodes = new HashSet<GraphNode<Student>>();

        foreach(var graph in graphs) {
            if (!visitedNodes.Contains(graph)) {
                result.Add(graph);
                foreach(var decendant in graph.AllDecendants()) {
                    visitedNodes.Add(decendant);
                }
            }
        }

        return result;
    }

    static List<String> FindNamesOfLargestGroups(List<GraphNode<Student>> graphs, Constraints constraints)
    {
        var distictGraphs = FindDistictGraphs(graphs).Where(g => g.AllDecendants().Count() >= constraints.MinSize).ToList();
        if(distictGraphs.Count == 0) {
            return new List<String>();
        }

        var lagestGroupSize = distictGraphs.Max(g => g.AllDecendants().Count);
        var largesGroups = distictGraphs.Where(g => g.AllDecendants().Count == lagestGroupSize).ToList();
        var names = largesGroups.SelectMany(g => g.AllDecendants()).OrderBy(s => s.Value.Name).Select(s => s.Value.Name).ToList();

        return names;
    }

    static void Main(string[] args)
    {
        string[] nmabfst = Console.ReadLine().Split(' ');

        int n = Convert.ToInt32(nmabfst[0]);

        int m = Convert.ToInt32(nmabfst[1]);

        int a = Convert.ToInt32(nmabfst[2]);

        int b = Convert.ToInt32(nmabfst[3]);

        int f = Convert.ToInt32(nmabfst[4]);

        int s = Convert.ToInt32(nmabfst[5]);

        int t = Convert.ToInt32(nmabfst[6]);

        membersInTheLargestGroups(n, m, a, b, f, s, t);
    }
}
