using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
namespace DBObject.IMS {

    public class Inventory {
        public static Inventory Default { get; set; } = new Inventory();

        public partial class InventoryModel
        {
        public long PackageId { get; set; } = -1;
        public string Image { get; set; } = "";
        public string PackageName { get; set; } = "";
        public int Category { get; set; } = 0;
        public string Description { get; set; } = "";
        public int StockIn { get; set; } = 0;
        public int StockOut { get; set; } = 0;
        public DateTime Date { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
        }

        public InventoryModel Save (InventoryModel package) 
        {
            var dc = new SQLLINQ.Models.InventoryManagementContext();
            var dbPackage = new SQLLINQ.Models.Inventory();

            if (package.PackageId == -1)
            {
                dbPackage.Image = package.Image;
                dbPackage.PackageName = package.PackageName;
                dbPackage.Category = package.Category;
                dbPackage.Description = package.Description;
                dbPackage.StockIn = package.StockIn;
                dbPackage.StockOut = package.StockOut;
                dbPackage.Date = package.Date;
                dc.Inventory.Add(dbPackage);
            }
            else
            { 
                dbPackage = dc.Inventory.Where(x => x.PackageId == package.PackageId).SingleOrDefault();
                if (dbPackage != null)
                {
                    dbPackage.Image = package.Image;
                    dbPackage.PackageName = package.PackageName;
                    dbPackage.Category = package.Category;
                    dbPackage.Description = package.Description;
                    dbPackage.StockIn = package.StockIn;
                    dbPackage.StockOut = package.StockOut;
                    dbPackage.Date = package.Date;
                }
            }
            dc.SaveChanges();    

            return package;
        }

        public static double getTotalItem()
        {
            var dc = new SQLLINQ.Models.InventoryManagementContext();
            var stock = dc.Inventory.Where(x => x.IsDeleted == false).Sum(x => x.StockIn);  
            return stock;
        }

        public InventoryModel StockIn (InventoryModel stock) 
        {
            var dc = new SQLLINQ.Models.InventoryManagementContext();
            var dbItem = new SQLLINQ.Models.Inventory();
            var payload = stock;

            dbItem = dc.Inventory.Where(x => x.PackageId == payload.PackageId).SingleOrDefault();
                if (dbItem != null)
                    {
                        dbItem.StockIn = dbItem.StockIn + payload.StockIn;
                    }
                dc.SaveChanges();

            return payload;
        }

        public InventoryModel StockOut (InventoryModel stock) 
        {
            var dc = new SQLLINQ.Models.InventoryManagementContext();
            var dbItem = new SQLLINQ.Models.Inventory();
            var payload = stock;

            dbItem = dc.Inventory.Where(x => x.PackageId == payload.PackageId).SingleOrDefault();
                if (dbItem != null)
                    {
                        dbItem.StockIn = dbItem.StockIn - payload.StockOut;
                        dbItem.StockOut = dbItem.StockOut + payload.StockOut;
                    }
                dc.SaveChanges();

            return payload;
        }

        public static new List<SQLLINQ.Models.Inventory> List() 
        {
            var dc = new SQLLINQ.Models.InventoryManagementContext();
            var dbInventory = dc.Inventory.Where(x => x.IsDeleted == false).ToList();
            var inventoryList = new List<SQLLINQ.Models.Inventory>();

            foreach (var inv in dbInventory)
            {
                var model = new SQLLINQ.Models.Inventory();
                model.PackageId = inv.PackageId;
                model.PackageName = inv.PackageName;
                model.Image = inv.Image;
                model.Category = inv.Category;
                model.Description = inv.Description;
                model.StockIn = inv.StockIn;
                model.StockOut = inv.StockOut;
                model.Date = inv.Date;
                inventoryList.Add(model);
            }
            return inventoryList.OrderBy(x => x.PackageName).ToList();
        }

        public static new List<SQLLINQ.Models.Inventory> ListDeleted() 
        {
            var dc = new SQLLINQ.Models.InventoryManagementContext();
            var dbInventory = dc.Inventory.Where(x => x.IsDeleted == true).ToList();
            var inventoryList = new List<SQLLINQ.Models.Inventory>();

            foreach (var inv in dbInventory)
            {
                var model = new SQLLINQ.Models.Inventory();
                model.PackageId = inv.PackageId;
                model.PackageName = inv.PackageName;
                model.Image = inv.Image;
                model.Category = inv.Category;
                model.Description = inv.Description;
                model.StockIn = inv.StockIn;
                model.StockOut = inv.StockOut;
                model.Date = inv.Date;
                inventoryList.Add(model);
            }
            return inventoryList.OrderBy(x => x.PackageName).ToList();
        }

        public static Boolean upload(Stream obj, string folder, string fileWithExtension)
        {
            bool result = false;
            //string path = Microsoft.AspNetCore.Http.HttpContext.Server.MapPath("~/App_Data/" + folder);
            string path = "./App_Data/" + folder;
            if (!File.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            using (var fileStream = new FileStream(path + "/" + fileWithExtension, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                obj.CopyTo(fileStream);
                fileStream.Dispose();
            }
            return result;
        }

        public static bool setDeleted(string id)
        {
            try
            {
                var dc = new SQLLINQ.Models.InventoryManagementContext();
                var dbFile = new SQLLINQ.Models.Inventory();

                dbFile = dc.Inventory.Where(x => x.PackageId == Convert.ToInt64(id)).SingleOrDefault();
                if (dbFile != null)
                    dbFile.IsDeleted = true;
                dc.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(InventoryModel id)
        {
            throw new NotImplementedException();
        }

        public static bool removeItem(string id)
        {
            try
            {
                var dc = new SQLLINQ.Models.InventoryManagementContext();
                var dbItem = new SQLLINQ.Models.Inventory();

                dbItem = dc.Inventory.Where(x => x.PackageId == Convert.ToInt64(id)).SingleOrDefault();
                if (dbItem != null)
                    dc.Remove(dbItem);
                dc.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool setRestore(string id)
        {
            try
            {
                var dc = new SQLLINQ.Models.InventoryManagementContext();
                var dbItem = new SQLLINQ.Models.Inventory();

                dbItem = dc.Inventory.Where(x => x.PackageId == Convert.ToInt64(id)).SingleOrDefault();
                if (dbItem != null)
                    dbItem.IsDeleted = false;
                dc.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Restore(InventoryModel id)
        {
            throw new NotImplementedException();
        }

    }
}