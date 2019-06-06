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
        [DllImport(@"D:\Программирование\Диплом\Бинарники\control.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ControlSystemEntryPoint(
            uint ID,
            IntPtr in_params,
            uint in_byte_count,
          out IntPtr out_params,
           out uint out_byte_count
  );

        [DllImport(@"D:\Программирование\Диплом\Бинарники\control.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ControlSystemGetErrorDescription(
    uint code,  
   out uint pSize
);



        static void Main(string[] args)
        {

            #region Загружаем модель в библиотеку и получаем имя модели "ТЕСТ"
            byte[] arrayModel = File.ReadAllBytes(@"D:\Программирование\Диплом\model\UploadModelIdParam-arch_ac.bin");
            IntPtr in_params = Marshal.AllocHGlobal(arrayModel.Length);
            Marshal.Copy(arrayModel, 0, in_params, arrayModel.Length);

            uint out_byte_count;
            IntPtr out_params;
            //   Marshal.FreeHGlobal(in_params);
            uint a = ControlSystemEntryPoint(2, in_params, (uint)arrayModel.Length, out out_params, out out_byte_count);

            byte[] nameOfModel = new byte[out_byte_count];
            Marshal.Copy(out_params, nameOfModel, 0, (int)out_byte_count); // копируем в nameOfModel имя модели
           
            #endregion

            #region Передаем в модель предметную область "arch_ac" и получаем в переменную b код ошибки 2752970754 0xa4170002
            
            byte[] subjectArray = Encoding.Default.GetBytes("arch_ac");
            byte[] subjectArrayLength = Encoding.Default.GetBytes($"{subjectArray.Length}");
            byte [] nameOfModelLength = Encoding.Default.GetBytes($"{nameOfModel.Length}");

            byte[] one = new byte[subjectArray.Length + subjectArrayLength.Length + nameOfModelLength.Length+ nameOfModel.Length];

            int k = 0;
            foreach (byte i in subjectArrayLength)
            {
                one[k] = i;
                k++;
            }           
            foreach (byte i in subjectArray)
            {
                one[k] = i;
                k++;
            }           
            foreach (byte i in nameOfModelLength)
            {
                one[k] = i;
                k++;
            }
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
