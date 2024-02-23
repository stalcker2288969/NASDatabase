using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASAPITests.DataBaseTests.Tools
{
    public class GenerateFile
    {
        [Fact]
        public void GenerateBMTest1()
        {
            if(!Directory.Exists("D:\\BMTest1"))
                Directory.CreateDirectory("D:\\BMTest1");
        }

        [Fact]
        public void DestroyBMTest1()
        {
            if (Directory.Exists("D:\\BMTest1"))
            {
                Directory.Delete("D:\\BMTest1", true);
            }
        }
    }
}
