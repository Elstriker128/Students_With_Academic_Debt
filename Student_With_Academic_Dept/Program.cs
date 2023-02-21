using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Student_With_Academic_Dept
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Faculty firstFaculty = InOutUtils.Read("FirstFaculty.txt");
            Faculty secondFaculty = InOutUtils.Read("SecondFaculty.txt");

            if(File.Exists("Rez.txt"))
                File.Delete("Rez.txt");

            InOutUtils.Print(firstFaculty, "Rez.txt", firstFaculty.faculty);
            InOutUtils.Print(secondFaculty, "Rez.txt", secondFaculty.faculty);  

            TaskUtils.Remove(firstFaculty);
            TaskUtils.Remove(secondFaculty);

            firstFaculty.Bubble();
            secondFaculty.Bubble();

            InOutUtils.Print(firstFaculty, "Rez.txt", firstFaculty.faculty);
            InOutUtils.Print(secondFaculty, "Rez.txt", secondFaculty.faculty);

            if(firstFaculty>secondFaculty)
            {
                File.AppendAllText("Rez.txt", $"{firstFaculty.faculty} yra daugiau studentų, turinčių akademines skolas");
            }
            else if(secondFaculty>firstFaculty)
            {
                File.AppendAllText("Rez.txt", $"{secondFaculty.faculty} yra daugiau studentų, turinčių akademines skolas");
            }
            else if(firstFaculty==secondFaculty)
            {
                File.AppendAllText("Rez.txt", $"Abiejuose fakultetuose yra vienodai studentų, turinčių akademines skolas");
            }
        }
    }
    class Student
    {
        public string Surname { get; private set; }
        public string Name { get; private set; }
        public string Group { get; private set; }
        private int[] Marks { get; set; }

        public Student(string surname, string name, string group, int[] marks)
        {
            Surname = surname;
            Name = name;
            Group = group;
            Marks = marks;
        }
        public int[] GetMarks() 
        { 
            return this.Marks; 
        }
        public int LessThan5(int ii)
        {
            if(ii<Marks.Count())
            {
                if (Marks[ii]<5)
                {
                    return 1+ LessThan5(ii + 1);
                }
                return LessThan5(ii+1);
            }
            else
            {
                return 0;
            }
        }
        public static bool operator >(Student a, Student b)
        {
            if(a.Surname!=b.Surname)
            {
                return string.Compare(a.Surname, b.Surname)>0;
            }
            else if(a.Name!=b.Name)
            {
                return string.Compare(a.Name, b.Name)>0;
            }
            else
            {
                return false;
            }
        }
        public static bool operator <(Student a, Student b)
        {
            if (a.Surname != b.Surname)
            {
                return string.Compare(a.Surname, b.Surname) < 0;
            }
            else if (a.Name != b.Name)
            {
                return string.Compare(a.Name, b.Name) < 0;
            }
            else
            {
                return false;
            }
        }
        public override string ToString()
        {
            return String.Format($"{this.Surname,-20} | {this.Name,-20} | {this.Group,-10} |");
        }
    }
    class Faculty
    {
        private List<Student> allStudents;
        public string faculty { get; private set; }
        public Faculty(string faculties)
        {
            allStudents = new List<Student>();
            this.faculty = faculties;
        }
        public void Add(Student student)
        {
            allStudents.Add(student);
        }
        public int Count()
        {
            return allStudents.Count();
        }
        public Student Get(int index)
        {
            return allStudents[index];
        }
        public static bool operator >(Faculty a, Faculty b)
        {
            return a.Count() > b.Count();
        }
        public static bool operator <(Faculty a, Faculty b)
        {
            return a.Count() < b.Count();
        }
        public static bool operator ==(Faculty a, Faculty b)
        {
            return a.Count() == b.Count();
        }
        public static bool operator !=(Faculty a, Faculty b)
        {
            return a.Count() != b.Count();
        }
        public void Bubble()
        {
            int i = 0;
            bool flag = true;
            while(flag)
            {
                flag = false;
                for (int j = allStudents.Count()-1; j > i; j--)
                {
                    if (allStudents[j] < allStudents[j-1])
                    {
                        flag = true;
                        var current = allStudents[j];
                        allStudents[j] = allStudents[j - 1];
                        allStudents[j - 1] = current;
                    }
                }
                i++;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Faculty faculty &&
                   EqualityComparer<List<Student>>.Default.Equals(allStudents, faculty.allStudents);
        }

        public override int GetHashCode()
        {
            return -233307388 + EqualityComparer<List<Student>>.Default.GetHashCode(allStudents);
        }
        public void Remove(int index)
        {
            allStudents.Remove(Get(index));
        }
    }
    class TaskUtils
    {
        public static void Remove(Faculty F)
        {
            for (int i = 0; i < F.Count(); i++)
            {
                Student current = F.Get(i);
                if(current.LessThan5(0)==0)
                {
                    F.Remove(i);
                    i--;
                }
            }
        }
    }
    class InOutUtils
    {
        public static Faculty Read(string Filename)
        {
            string[] lines = File.ReadAllLines(Filename);
            string facultyName = Regex.Match(lines[0], $@"([\w]+)").Value;
            Faculty allStudents = new Faculty(facultyName);
            foreach (string line in lines.Skip(1))
            {
                string[] values = Regex.Split(line, ", ");
                string surname = values[0];
                string name = values[1];
                string group = values[2];
                int[] marks = new int[values.Length-3];
                for (int i = 0; i < values.Count()-3; i++)
                {
                    marks[i] = int.Parse(values[3+i]);
                }
                Student current = new Student(surname, name, group, marks);
                allStudents.Add(current);
            }
            return allStudents;
        }
        public static void Print(Faculty F, string Filename, string Header)
        {
            string[] lines = new string[F.Count()+5];
            lines[0] = String.Format(new string('-', 75));
            lines[1] = String.Format($"{Header,-20}");
            lines[2] = String.Format(new string('-', 75));
            for (int i = 0; i < F.Count(); i++)
            {
                Student current = F.Get(i);
                lines[3 + i] = current.ToString();
                int[] curMarks = current.GetMarks();
                for (int j = 0; j < curMarks.Count(); j++)
                {
                    lines[3 + i] += String.Format($"{curMarks[j],-2}");
                }
            }
            lines[F.Count() + 3] = String.Format(new string('-', 75));
            File.AppendAllLines(Filename, lines, Encoding.UTF8);

        }
    }
}
