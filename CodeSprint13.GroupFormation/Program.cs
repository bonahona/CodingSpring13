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

    class Pair
    {
        public Student First  { get; set; }
        public Student Second { get; set; }

        public List<Student> GetMembers() => new List<Student> { First, Second };
    }

    class Group
    {
        public List<Student> Members { get; set; }

        public Group()
        {
            Members = new List<Student>();
        }

        public bool Contains(Student student) => Members.Contains(student);
        public void Add(Student student) => Members.Add(student);
        public int GetCountForGrade(int grade) => Members.Where(m => m.Grade == grade).Count();
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

    // Complete the membersInTheLargestGroups function below.
    static void membersInTheLargestGroups(int n, int m, int a, int b, int f, int s, int t)
    {
        var contraints = new Constraints(a, b, f, s, t);

        var students = ReadStudents(n);
        var groups = ReadRequests(m, students, contraints);

        var validGroups = groups.Where(g => g.Members.Count >= contraints.MinSize).Where(g => g.Members.Count == groups.Max(c => c.Members.Count)).ToList();

        var studentInLargestGroup = validGroups.SelectMany(g => g.Members).OrderBy(w => w.Name).ToList();

        if(studentInLargestGroup.Count == 0) {
            Console.WriteLine("no groups");
        }else{
            foreach(var student in studentInLargestGroup) {
                Console.WriteLine(student.Name);
            }
        }
        Console.ReadLine();

    }

    static Dictionary<String,Student> ReadStudents(int n)
    {
        var result = new Dictionary<String, Student>();
        for(int i = 0; i < n; i++) {
            var student = Console.ReadLine().Split(' ');
            result.Add(student[0], new Student { Name = student[0], Grade = Convert.ToInt32(student[1]) });
        }

        return result;
    }

    static List<Group> ReadRequests(int requestCount, Dictionary<String, Student> students, Constraints contraints)
    {
        var result = new List<Group>();

        for(int i = 0; i < requestCount; i++) {
            var request = Console.ReadLine().Split(' ');
            HandleRequest(new Pair { First = students[request[0]], Second = students[request[1]] }, result, contraints);
        }

        return result;
    }

    static void HandleRequest(Pair pair, List<Group> groups, Constraints constraints)
    {
        var contaningGroups = FindContainingGroups(pair, groups);

        if(contaningGroups.Item1 == null && contaningGroups.Item2 == null) {
            var group = new Group();
            group.Add(pair.First);
            group.Add(pair.Second);
            groups.Add(group);
        }else if(contaningGroups.Item1 == contaningGroups.Item2) {
            return;
        }else if(contaningGroups.Item1 == null) {
            if(StudentFitsIntoGroup(contaningGroups.Item2, pair.First, constraints)) {
                contaningGroups.Item2.Add(pair.First);
            }
        }else if(contaningGroups.Item2 == null) {
            if (StudentFitsIntoGroup(contaningGroups.Item1, pair.Second, constraints)) {
                contaningGroups.Item1.Add(pair.Second);
            }
        } else {
            if (MergeFits(contaningGroups.Item1, contaningGroups.Item2, constraints)){
                var group = MergeGroups(contaningGroups.Item1, contaningGroups.Item2);
                groups.Add(group);
                contaningGroups.Item1.Members.Clear();
                contaningGroups.Item2.Members.Clear();
            } 
        }
    }

    static bool StudentFitsIntoGroup(Group group, Student student, Constraints constraints)
    {
        if(group.Members.Count >= constraints.MaxSize) {
            return false;
        }

        if(group.GetCountForGrade(student.Grade) >= constraints.GradeSize[student.Grade]) {
            return false;
        }

        return true;
    }

    static bool MergeFits(Group group1, Group group2, Constraints constraints)
    {
        if(group1.Members.Count + group2.Members.Count > constraints.MaxSize) {
            return false;
        }

        for(int i = 1; i <= 3; i++) {
            if(!MergeGradeFits(group1, group2, i, constraints)) {
                return false;
            }
        }

        return true;
    }

    static bool MergeGradeFits(Group group1, Group group2, int grade, Constraints constraints) => group1.GetCountForGrade(grade) + group2.GetCountForGrade(grade) <= constraints.GradeSize[grade];

    static Group MergeGroups(Group group1, Group group2)
    {
        var result = new Group();

        foreach(var member in group1.Members) {
            result.Add(member);
        }

        foreach (var member in group2.Members) {
            result.Add(member);
        }

        return result;
    }

    static Tuple<Group, Group> FindContainingGroups(Pair pair, List<Group> groups) => new Tuple<Group, Group>(FindContainingGroup(pair.First, groups), FindContainingGroup(pair.Second, groups));

    static Group FindContainingGroup(Student student, List<Group> groups)
    {
        foreach(var group in groups) {
            if (group.Contains(student)){
                return group;
            }
        }

        return null;
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
