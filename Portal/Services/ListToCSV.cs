using System.Reflection;

namespace Portal.Services
{
    public class ListToCSV<T>
    {
        private int i = 1;
        private Type _type;
        private List<T> _list;
        private string[] _headers;
        public ListToCSV(List<T> list, string[] headers)
        {
            _list = list;
            _headers = headers;
            _type = typeof(T);
        }

        private byte[] CreateCSVBody()
        {
            var fields = _type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            using (var ms = new MemoryStream())
            {

                using (TextWriter sr = new StreamWriter(ms))
                {
                    foreach (var list in _list)
                    {

                        sr.Write(i++ + ";");
                        foreach (var field in fields)
                        {
                            sr.Write(field.GetValue(list).ToString() + ";");
                        }
                        sr.Write(Environment.NewLine);
                    }
                    sr.Write(";Итого:;");
                    sr.Flush();
                }
                return ms.ToArray();
            }

        }

        private byte[] CreateCSVFooter()
        {
            var fields = _type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            using (var ms = new MemoryStream())
            {
                using (TextWriter sr = new StreamWriter(ms))
                {
                    foreach (var field in fields)
                    {
                        int sum;
                        decimal Sum;
                        Type _t = field.GetType();
                        if (field.GetType() == typeof(DateTime)) break;
                        if (field.FieldType == typeof(int))
                        {
                            sum = _list.Sum(x => (int)field.GetValue(x));
                            sr.Write(sum.ToString() + ";");
                        }
                        if (field.FieldType == typeof(decimal))
                        {
                            Sum = _list.Sum(x => (decimal)field.GetValue(x));
                            sr.Write(Sum.ToString() + ";");
                        }

                    }
                    sr.Flush();
                    return ms.ToArray();
                }

            }
        }

        private byte[] CreateCSVHeader()
        {

            using (var ms = new MemoryStream())
            {

                using (TextWriter sr = new StreamWriter(ms, System.Text.Encoding.UTF8))
                {
                    foreach (var header in _headers)
                    {

                        sr.Write(header + ";");
                    }
                    sr.Write(Environment.NewLine);
                    sr.Flush();
                }
                return ms.ToArray();
            }

        }

        public byte[] CreateCSV()
        {
            byte[] header = CreateCSVHeader();
            byte[] body = CreateCSVBody();
            byte[] footer = CreateCSVFooter();
            byte[] file = new byte[header.Length + body.Length + footer.Length];
            header.CopyTo(file, 0);
            body.CopyTo(file, header.Length);
            footer.CopyTo(file, body.Length + header.Length);
            return file;
        }
    }
}