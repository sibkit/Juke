using System;
using System.IO;
using BoardFlow.Formats.Excellon.Reading;
using BoardFlow.Formats.Gerber.Reading;

namespace BoardFlowTest;

public static class Reader
{
    private static void TestExcellon() {
        var di = new DirectoryInfo(Directory.GetCurrentDirectory() + @"\test_files\excellon");
        var files = di.GetFiles("*");

        foreach (var fi in files) {

            Console.Write("Обработка файла: " + fi.Name);
            try {
                var p = ExcellonReader.Instance.Read(fi);
                if (p.Item2.Errors.Count > 0) {
                    Console.WriteLine("  ...ERROR");
                    Console.WriteLine("Обнаружены ошибки:");
                    foreach (var error in p.Item2.Errors) {
                        Console.WriteLine(error);
                    }

                    Console.WriteLine();
                }

                if (p.Item2.Warnings.Count > 0) {
                    Console.WriteLine("Обнаружены предупреждения:");
                    foreach (var warning in p.Item2.Warnings) {
                        Console.WriteLine(warning);
                    }
                }

                if (p.Item2.Errors.Count == 0 && p.Item2.Warnings.Count == 0) {
                    Console.WriteLine("  ...OK");
                }
            } catch (ApplicationException e) {
                //Console.WriteLine("");
                Console.WriteLine("Не удалось прочитать файл: \"" + fi.Name + "\"");
                Console.WriteLine(e);
                //Console.WriteLine("");
            }
            //Console.WriteLine("");
        }
    }

    private static void TestGerber() {
        var di = new DirectoryInfo(Directory.GetCurrentDirectory() + @"\test_files\gerber\03");
        var files = di.GetFiles("*");
        var i = 0;
        foreach (var fi in files) {
            // if(!fi.Name.Contains("COMPMASK"))
            //     continue;
            // if(i>3)
            //     break;
            // i++;
            if(fi.Extension.ToUpper() is ".DRL" or ".TC" or ".TOL")
                continue;
            Console.WriteLine("-----");
            Console.WriteLine("Обработка файла: " + fi.Name);
            try {
                var p = GerberReader.Instance.Read(fi);
                if (p.Item2.Errors.Count > 0) {
                    Console.WriteLine("  ...ERROR");
                    Console.WriteLine("Обнаружены ошибки:");
                    foreach (var error in p.Item2.Errors) {
                        Console.WriteLine(error);
                    }

                    Console.WriteLine();
                }

                if (p.Item2.Warnings.Count > 0) {
                    Console.WriteLine("Обнаружены предупреждения:");
                    foreach (var warning in p.Item2.Warnings) {
                        Console.WriteLine(warning);
                    }
                }

                if (p.Item2.Errors.Count == 0 && p.Item2.Warnings.Count == 0) {
                    Console.WriteLine("  ...OK");
                }
            } catch (ApplicationException e) {
                //Console.WriteLine("");
                Console.WriteLine("Не удалось прочитать файл: \"" + fi.Name + "\"");
                Console.WriteLine(e);
                
            }
            Console.WriteLine("-----");
            Console.WriteLine("");
        }
    }
    public static void TestReader()
    {

        TestGerber();
    }
}