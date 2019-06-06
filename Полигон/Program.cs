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
              Console.WriteLine(Encoding.UTF8.GetString(nameOfModel));
            #endregion

            #region Передаем в модель предметную область "arch_ac" и получаем в переменную b код ошибки 2752970754 0xa4170002
            string subject = "arch_ac";
            byte[] subjectArray = Encoding.Default.GetBytes(subject);
             in_params = Marshal.AllocHGlobal(subjectArray.Length);
            Marshal.Copy(subjectArray, 0, in_params, subjectArray.Length);

         
            //   Marshal.FreeHGlobal(in_params);
            uint b = ControlSystemEntryPoint(3, in_params, (uint)subjectArray.Length, out out_params, out out_byte_count);

            byte[] temp2 = new byte[out_byte_count];
            Marshal.Copy(out_params, nameOfModel, 0, (int)out_byte_count);
              Console.WriteLine(Encoding.UTF8.GetString(temp2));
            Console.ReadKey();
            #endregion

            #region дальше, если я правильно понимаю, то в случае успешного выполнения  b = 0 и тогда передаем подуправляющей системе имя модели - ТЕСТ, оно записано в массив nameOfModel
            
          
            in_params = Marshal.AllocHGlobal(nameOfModel.Length);
            Marshal.Copy(nameOfModel, 0, in_params, nameOfModel.Length);


            //   Marshal.FreeHGlobal(in_params);
            uint c = ControlSystemEntryPoint(3, in_params, (uint)nameOfModel.Length, out out_params, out out_byte_count);
            // с = 2752970754 0xa4170002

            byte[] temp3 = new byte[out_byte_count]; 
           
            Marshal.Copy(out_params, temp3, 0, (int)out_byte_count);
            // при с = 2752970754 0xa4170002  в temp3 будет имя "ТЕСТ"



            #endregion

        }




    }
}
