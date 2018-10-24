using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ShibaStoreService
{
    public partial class Service1 : ServiceBase
    {
        private FileSystemWatcher watcher;


        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

            watcher = new FileSystemWatcher();
            watcher.Path = @"C:\Users\wrenq\source\repos\ShibaStoreService\ShibaStoreService\DataFolder\";
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                   | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "*.*";
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
        }

        protected override void OnStop()
        {

        }

        protected void OnChanged(object source, FileSystemEventArgs e)
        {
            StoreDBEntities db = new StoreDBEntities();

            StreamReader reader = new StreamReader(e.FullPath);

           
            
            using (reader)
            {

                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    try
                    {
                        string[] entry = line.Split(new char[] { '|' });


                        string dogname = entry[0].Trim();
                        string dogdesc = entry[1].Trim();

                        string dogimage = entry[2].Trim();


                        bool dogExists = db.Dogs.Any(x => x.DogName == dogname && x.DogDesc == dogdesc && x.ImageName == dogimage);

                        if (dogExists == false)
                        {
                            Dog dog = new Dog();

                            dog.DogName = dogname;
                            dog.DogDesc = dogdesc;
                            dog.ImageName = dogimage;




                            db.Dogs.Add(dog);
                            db.SaveChanges();
                        }
                    }

                    catch (Exception ex)
                    {
                        DBError dberror = new DBError();
                        dberror.ErrorText = ex.ToString();
                        db.DBErrors.Add(dberror);
                        db.SaveChanges();
                    }





                }
            }
        }


    }
}
