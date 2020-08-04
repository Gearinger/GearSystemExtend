using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI;
using NPOI.HPSF;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace GearConfig
{
    public class GearConfig
    {
        public static string DefaultPath { get; set; } = AppDomain.CurrentDomain.BaseDirectory;

        public static List<T> GetConfigModelList<T>(string name) where T : class, new()
        {
            List<T> list = new List<T>();

            var props = typeof(T).GetProperties();

            var excelData = ReadExcelData(name);
            var rows = excelData.Rows;

            if (excelData.Columns.Count != props.Count())
            {
                throw new Exception($"配置文件{name}格式存在问题！");
            }

            for (int i = 0; i < rows.Count; i++)
            {
                var model = new T();
                for (int j = 0; j < props.Count(); j++)
                {
                    props[j].SetValue(model, rows[i][j].ToString());
                }
                list.Add(model);
            }

            return list;
        }

        public static DataTable ReadExcelData(string name)
        {
            
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            var workbook_exc = new XSSFWorkbook(DefaultPath + name + ".xlsx");
            var worksheet = workbook_exc.GetSheetAt(0);

            DataTable dataTable = ExcelToDataTable(worksheet, true);

            return dataTable;
        }

        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <returns>返回的DataTable</returns>
        public static DataTable ExcelToDataTable(ISheet sheet, bool isFirstRowColumn)
        {
            DataTable data = new DataTable();
            try
            {
                if (sheet != null)
                {
                    int startRow = 0;
                    IRow firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            ICell cell = firstRow.GetCell(i);
                            if (cell != null)
                            {
                                string cellValue = cell.StringCellValue;
                                if (!string.IsNullOrEmpty(cellValue))
                                {
                                    DataColumn column = new DataColumn(cellValue);
                                    data.Columns.Add(column);
                                }
                            }
                        }
                        startRow = sheet.FirstRowNum + 1;
                    }
                    else
                    {
                        startRow = sheet.FirstRowNum;
                    }

                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                                dataRow[j] = row.GetCell(j).ToString();
                        }
                        data.Rows.Add(dataRow);
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return null;
            }
        }
    }
}
