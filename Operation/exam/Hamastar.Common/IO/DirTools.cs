using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace Hamastar.Common.IO
{
    public class DirTools
    {
        public static long DirSize(DirectoryInfo d)
        {
            long Size = 0;

            FileInfo[] files = d.GetFiles();
            foreach (FileInfo file in files)
            {
                Size += file.Length;
            }

            DirectoryInfo[] dirs = d.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                Size += DirSize(dir);
            }
            return (Size);
        }

        public static void DeleteAll(DirectoryInfo d)
        {
            if (d.Exists)
            {
                foreach (FileInfo fileinfo in d.GetFiles())
                {
                    fileinfo.Delete();
                }


                foreach (DirectoryInfo dirinfo in d.GetDirectories())
                {
                    DeleteAll(dirinfo);
                    //dirinfo.Delete();
                }

                d.Delete();
            }
        }

        /// <summary>
        /// 複製或刪除目錄
        /// </summary>
        /// <param name="strSourceDir"></param>
        /// <param name="strDestDir"></param>
        /// <param name="bDelSource"></param>
        /// <param name="IsOverwrite"></param>
        public static void MoveDirectory(string strSourceDir, string strDestDir, bool bDelSource, bool IsOverwrite)
        {
            if (Directory.Exists(strSourceDir))
            {
                try
                {
                    CopyDirectory(new DirectoryInfo(strSourceDir), new DirectoryInfo(strDestDir), IsOverwrite);
                    if (bDelSource)
                    {
                        Directory.Delete(strSourceDir, true);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        //private void CopyDirectory(DirectoryInfo diSourceDir, DirectoryInfo diDestDir)
        //{
        //    if (!diDestDir.Exists)
        //    {
        //        diDestDir.Create();
        //    }
        //    FileInfo[] fiSrcFiles = diSourceDir.GetFiles();
        //    foreach (FileInfo fiSrcFile in fiSrcFiles)
        //    {
        //        fiSrcFile.CopyTo(Path.Combine(diDestDir.FullName, fiSrcFile.Name), true);
        //    }
        //    DirectoryInfo[] diSrcDirectories = diSourceDir.GetDirectories();
        //    foreach (DirectoryInfo diSrcDirectory in diSrcDirectories)
        //    {
        //        CopyDirectory(diSrcDirectory, new DirectoryInfo(Path.Combine(diDestDir.FullName, diSrcDirectory.Name)));
        //    }
        //}


        private static void CopyDirectory(DirectoryInfo diSourceDir, DirectoryInfo diDestDir, bool IsOverwrite)
        {
            string DestDirName = string.Empty;

            if (diDestDir.Exists)
            {
                if (IsOverwrite)
                {
                    DestDirName = diDestDir.FullName;
                }
                else
                {
                    //DestDirName = FileTools.Rename(diDestDir.FullName);
                    DestDirName = diDestDir.Parent.FullName + @"\" + GetNewDirName(diDestDir.FullName);
                    DirectoryInfo NewDir = new DirectoryInfo(DestDirName);
                    if (!NewDir.Exists)
                    {
                        NewDir.Create();
                    }
                }
            }
            else
            {
                diDestDir.Create();
                DestDirName = diDestDir.FullName;
            }

            foreach (FileInfo fiSrcFile in diSourceDir.GetFiles())
            {
                FileInfo fiDestFile = new FileInfo(Path.Combine(DestDirName, fiSrcFile.Name));

                if (fiDestFile.Exists)
                {
                    if (IsOverwrite)
                    {
                        fiSrcFile.CopyTo(Path.Combine(DestDirName, fiSrcFile.Name), true);
                    }
                    else
                    {
                        //string NewFileName = FileTools.Rename(Path.GetFileNameWithoutExtension(fiDestFile.Name)) + Path.GetExtension(fiDestFile.Name).ToLower();
                        string NewFileName = FileTools.GetNewFileName(Path.Combine(DestDirName, Path.GetFileNameWithoutExtension(fiDestFile.Name)), Path.GetExtension(fiDestFile.Name).ToLower()) + Path.GetExtension(fiDestFile.Name).ToLower();
                        NewFileName = Path.Combine(DestDirName, NewFileName);
                        fiDestFile.MoveTo(NewFileName);
                    }
                }
                else
                {
                    fiSrcFile.CopyTo(Path.Combine(DestDirName, fiSrcFile.Name), true);
                }
            }

            foreach (DirectoryInfo diSrcDirectory in diSourceDir.GetDirectories())
            {
                CopyDirectory(diSrcDirectory, new DirectoryInfo(Path.Combine(DestDirName, diSrcDirectory.Name)), IsOverwrite);
            }
        }

        private static string NewDirName = string.Empty;
        public static string GetNewDirName(string OldDirName)
        {
            DirectoryInfo FileInfo = new DirectoryInfo(OldDirName);
            if (!FileInfo.Exists)
            {
                NewDirName = Path.GetFileName(OldDirName);
            }
            else
            {
                GetNewDirName(FileTools.Rename(OldDirName));
            }

            return NewDirName;
        }

        # region 目錄排序
        public class MyDirSorter : IComparer
        {
            #region IComparer Members
            public int Compare(object x, object y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }
                if (x == null)
                {
                    return -1;
                }
                if (y == null)
                {
                    return 1;
                }
                DirectoryInfo xInfo = (DirectoryInfo)x;
                DirectoryInfo yInfo = (DirectoryInfo)y;


                //依名稱排序
                return xInfo.FullName.CompareTo(yInfo.FullName);//遞增
                //return yInfo.FullName.CompareTo(xInfo.FullName);//遞減

                //依修改日期排序
                //return xInfo.LastWriteTime.CompareTo(yInfo.LastWriteTime);//遞增
                //return yInfo.LastWriteTime.CompareTo(xInfo.LastWriteTime);//遞減
            }
            #endregion
        }
        # endregion
    }


}
