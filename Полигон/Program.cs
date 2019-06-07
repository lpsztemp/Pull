using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Permissions;
using System.Security;
using System.Runtime.InteropServices;

namespace Полигон
{


    class Program
    {
        [DllImport(@"D:\temp\pull_input\control.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ControlSystemEntryPoint(
            UInt32 ID,
            IntPtr in_params,
            UInt32 in_byte_count,
          out IntPtr out_params,
           out UInt32 out_byte_count
  );

        [DllImport(@"D:\temp\pull_input\control.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ControlSystemGetErrorDescription(
    uint code,  
   out uint pSize
);



        static void Main(string[] args)
        {

            #region Загружаем модель в библиотеку и получаем имя модели "ТЕСТ"
            byte[] arrayModel = File.ReadAllBytes(@"D:\temp\pull_input\arch_ac_model.xml.bin");
            IntPtr in_params = Marshal.AllocHGlobal(arrayModel.Length);
            Marshal.Copy(arrayModel, 0, in_params, arrayModel.Length);

            UInt32 out_byte_count;
            IntPtr out_params;
            //   Marshal.FreeHGlobal(in_params);
            uint a = ControlSystemEntryPoint(2, in_params, (UInt32) arrayModel.Length, out out_params, out out_byte_count);

            byte[] nameOfModel = new byte[out_byte_count];
            Marshal.Copy(out_params, nameOfModel, 0, (int)out_byte_count); // копируем в nameOfModel имя модели
           
            #endregion

            #region Передаем в модель предметную область "arch_ac" и получаем в переменную b код ошибки 2752970754 0xa4170002
            
            byte[] subjectArray = Encoding.Default.GetBytes("arch_ac");
            byte[] subjectArrayLength = Encoding.Default.GetBytes($"{subjectArray.Length}");
            byte [] nameOfModelLength = Encoding.Default.GetBytes($"{nameOfModel.Length}");

            byte[] one = new byte[subjectArray.Length + 4 + 4 + nameOfModel.Length];

            /*Четыре байта - длина модели, байты [00 00 00 04], а не как у вас [37]. Найдете автоматический запаковщик в
             * ПОСЛЕДОВАТЕЛЬНОСТЬ БАЙТ (а не текста), можете использовать его. */
            one[0] = 7;
            one[1] = 0;
            one[2] = 0;
            one[3] = 0;
            int k = 4;
            foreach (byte i in subjectArray)
            {
                one[k] = i;
                k++;
            }
            UInt32 val = (UInt32)nameOfModel.Length;
            one[k++] = (byte) (val & 0xff);
            one[k++] = (byte) ((val >> 8) & 0xff);
            one[k++] = (byte) ((val >> 16) & 0xff);
            one[k++] = (byte) ((val >> 24) & 0xff);
            foreach (byte i in nameOfModel)
            {
                one[k] = i;
                k++;
            }

            in_params = Marshal.AllocHGlobal(one.Length);
            Marshal.Copy(one, 0, in_params, one.Length);

         
            //   Marshal.FreeHGlobal(in_params);
            uint b = ControlSystemEntryPoint(3, in_params, (uint)one.Length, out out_params, out out_byte_count);
            uint pSize;
            IntPtr error = ControlSystemGetErrorDescription(b, out pSize);
            var result =  Marshal.PtrToStringUTF8(error);
            byte[] nameOfProcess = new byte[out_byte_count];
            Marshal.Copy(out_params, nameOfModel, 0, (int)out_byte_count);
              Console.WriteLine(Encoding.UTF8.GetString(nameOfProcess));
            Console.ReadKey();
            #endregion

         

        }




    }
}
