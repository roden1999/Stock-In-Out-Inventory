using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers {
    [Route ("[controller]/[action]")]
    [ApiController]
    public class InventoryController : ControllerBase {

        private DBObject.IMS.Inventory.InventoryModel DBObjectInstance()
        {
            return new DBObject.IMS.Inventory.InventoryModel();
        }

        [HttpGet]
        public ActionResult avatarImageByFileName(string filename)
        {
            var defaultImage = new FileStream("./App_Data/default.jpg", FileMode.Open, FileAccess.Read, FileShare.Read);
            var buffer = new FileStream("./App_Data/default.jpg", FileMode.Open, FileAccess.Read, FileShare.Read);
            var path = "./App_Data/Inventory/"+ filename;
            if (System.IO.File.Exists(path))
            {
                    var fileStream = new FileStream("./App_Data/Inventory/" + filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(fileStream, "image/jpg");
            }

            return File(defaultImage, "image/png");
        }

        public static string GetFileExtension(string base64String)
        {
            var data = base64String.Substring(0, 5);

            switch (data.ToUpper())
            {
                case "IVBOR":
                    return "png";
                case "/9J/4":
                    return "jpg";
                case "AAAAF":
                    return "mp4";
                case "JVBER":
                    return "pdf";
                case "AAABA":
                    return "ico";
                case "UMFYI":
                    return "rar";
                case "E1XYD":
                    return "rtf";
                case "U1PKC":
                    return "txt";
                case "MQOWM":
                case "77U/M":
                    return "srt";
                default:
                    return string.Empty;
            }
        }

        [HttpPost]
        public ActionResult Save () {
            var stream = HttpContext.Request.Body;

            using (var reader = new System.IO.StreamReader(stream)) {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<DBObject.Common.RequestClass<DBObject.IMS.Inventory.InventoryModel>>(reader.ReadToEnd());

                var id = result.Payload.PackageId;
                var imageData = result.Payload.Image;
                if (!string.IsNullOrEmpty(imageData))
                {
                     var file = imageData.Split(';')[1].Split(',')[1];
                     var bytes = Convert.FromBase64String(file);
                     var content = new MemoryStream(bytes);
                     var uniqueName = Guid.NewGuid();
                     var fileName = uniqueName + "." + GetFileExtension(file);
                     result.Payload.Image = fileName;
                     DBObject.IMS.Inventory.upload(content, "Inventory", fileName);
                }

                var company =  DBObject.IMS.Inventory.Default.Save(result.Payload);

                // create object without creating class
                var rc = new {
                    Status = "OK",
                    JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(company)
                };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(rc), "application/json");
            }
        }

        [HttpPost]
        public ActionResult StockIn(){
            using (StreamReader reader = new StreamReader(this.HttpContext.Request.Body, Encoding.UTF8)) {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<DBObject.Common.RequestClass<DBObject.IMS.Inventory.InventoryModel>>(reader.ReadToEnd());
                
                var stock =  DBObject.IMS.Inventory.Default.StockIn(result.Payload);

                var rc = new {
                    Status = "OK",
                    JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(stock)
                };
                return Content(JsonConvert.SerializeObject(rc), "application/json");
            }
        }

        [HttpPost]
        public ActionResult StockOut(){
            using (StreamReader reader = new StreamReader(this.HttpContext.Request.Body, Encoding.UTF8)) {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<DBObject.Common.RequestClass<DBObject.IMS.Inventory.InventoryModel>>(reader.ReadToEnd());
                
                var stock =  DBObject.IMS.Inventory.Default.StockOut(result.Payload);

                var rc = new {
                    Status = "OK",
                    JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(stock)
                };
                return Content(JsonConvert.SerializeObject(rc), "application/json");
            }
        }

        [HttpPost]
        public ActionResult totalStock(){
            using (StreamReader reader = new StreamReader(this.HttpContext.Request.Body, Encoding.UTF8)) {

                var total = Newtonsoft.Json.JsonConvert.SerializeObject(DBObject.IMS.Inventory.getTotalItem());
                var rc = new {
                    Status = "OK",
                    JsonData = total
                };
                return Content(JsonConvert.SerializeObject(rc), "application/json");
            }
        }
        
        [HttpPost]
        public ActionResult PackageList () {
            using (StreamReader reader = new StreamReader(this.HttpContext.Request.Body, Encoding.UTF8)) {
                var list = Newtonsoft.Json.JsonConvert.SerializeObject(DBObject.IMS.Inventory.List());

                var rc = new {
                    Status = "OK",
                    JsonData = list
                };
                return Content(JsonConvert.SerializeObject(rc, Formatting.Indented), "application/json");
            }
        }

        [HttpPost]
        public ActionResult PackageListDeleted () {
            using (StreamReader reader = new StreamReader(this.HttpContext.Request.Body, Encoding.UTF8)) {
                var list = Newtonsoft.Json.JsonConvert.SerializeObject(DBObject.IMS.Inventory.ListDeleted());

                var rc = new {
                    Status = "OK",
                    JsonData = list
                };
                return Content(JsonConvert.SerializeObject(rc, Formatting.Indented), "application/json");
            }
        }

        [HttpPost]
        public ActionResult Remove() {
            var stream = HttpContext.Request.Body;

            using (var reader = new System.IO.StreamReader (stream)) {
                
                var value = Newtonsoft.Json.JsonConvert.DeserializeObject<DBObject.Common.RequestClass<long>>(reader.ReadToEnd());
                var id = value.Payload;
                var delete = DBObject.IMS.Inventory.removeItem(id.ToString());

                // create object without creating class
                var rc = new {
                    Status = "OK",
                    JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(delete)
                };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(rc));
            }
        }

        [HttpPost]
        public ActionResult RestoreFile() {
            var stream = HttpContext.Request.Body;

            using (var reader = new System.IO.StreamReader (stream)) {
                
                var value = Newtonsoft.Json.JsonConvert.DeserializeObject<DBObject.Common.RequestClass<long>>(reader.ReadToEnd());
                var id = value.Payload;
                var restore = DBObject.IMS.Inventory.setRestore(id.ToString());

                // create object without creating class
                var rc = new {
                    Status = "OK",
                    JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(restore)
                };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(rc));
            }
        }

        [HttpPost]
        public ActionResult DeletePackage() {
            var stream = HttpContext.Request.Body;

            using (var reader = new System.IO.StreamReader (stream)) {
                
                var value = Newtonsoft.Json.JsonConvert.DeserializeObject<DBObject.Common.RequestClass<long>>(reader.ReadToEnd());
                var id = value.Payload;
                var delete = DBObject.IMS.Inventory.setDeleted(id.ToString());

                // create object without creating class
                var rc = new {
                    Status = "OK",
                    JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(delete)
                };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(rc));
            }
        }

    }
}