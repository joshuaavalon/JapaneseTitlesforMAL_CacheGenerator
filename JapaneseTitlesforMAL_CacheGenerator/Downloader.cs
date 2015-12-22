using System;
using System.IO;
using System.Threading.Tasks;

namespace JapaneseTitlesforMAL_CacheGenerator
{
    public class Downloader
    {
        public delegate void ProgressEvent(string progress);

        private readonly int _id;
        private readonly Type _type;
        private readonly string _typeName;
        private string[] _names;

        public Downloader(Type type, int id)
        {
            _id = id;
            _type = type;
            switch (type)
            {
                case Type.Anime:
                    _typeName = "anime";
                    break;
                case Type.Manga:
                    _typeName = "manga";
                    break;
            }
        }

        public event ProgressEvent OnProgress = progress => { };

        public void Start()
        {
            if (_id < 1) return;
            _names = new string[_id];
            Parallel.For(1, _id + 1, i =>
            {
                var interpreter = new Interpreter(_type, i);
                _names[i - 1] = interpreter.GetJapaneseTitle();
                OnProgress($"{i}: {_names[i - 1]}");
            });

            string fileName;
            var repeat = 0;
            do
            {
                fileName = GetFileName(repeat);
                repeat++;
            } while (File.Exists(fileName));

            using (var writetext = new StreamWriter(fileName))
            {
                writetext.WriteLine(
                    $"# Japanese titles for {_typeName} on myanimelist.net. Line number corresponds to MAL ID plus 1. Last updated:" +
                    DateTime.Now.ToString("MM/dd/yyyy"));
                foreach (var name  in _names)
                    writetext.WriteLine(name);
            }
        }

        private string GetFileName(int repeat = 0)
        {
            var name = _typeName;
            if (repeat != 0)
                name += $" ({repeat})";
            name += ".txt";
            return name;
        }
    }
}