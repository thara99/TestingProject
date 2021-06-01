using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TestingProject.Models;

namespace TestingProject.Controllers
{
    public class StudentController : Controller
    {

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "slmoEsLuT4TwEcsd5oOu6gsy5nbQtyG7JCXLOMvI",
            BasePath = "https://asp-mvc-b412a-default-rtdb.firebaseio.com/"
        };
        IFirebaseClient client;
        // GET: Student
        public ActionResult Index()
        {

            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Student");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Student>();
            foreach(var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Student>(((JProperty)item).Value.ToString()));
            }
            return View(list);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Student student)
        {
            try
            {
                AddStudentToFirebase(student);
                ModelState.AddModelError(String.Empty, "Added Successfully");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(String.Empty, ex.Message);
            }
            return View();
        }
      
            
        

        private void AddStudentToFirebase(Student student)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = student;
            PushResponse response = client.Push("Student/", data);
            data.student_id = response.Result.name;
            SetResponse setResponse = client.Set("Student/" + data.student_id, data);
        }

        [HttpGet]
        public ActionResult Detail(String id)
        {

            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Student/"+id); 
            dynamic data = JsonConvert.DeserializeObject<Student>(response.Body);
            return View(data);
        }

        [HttpGet]
        public ActionResult Edit(String id)
        {

            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Student/" + id);
            dynamic data = JsonConvert.DeserializeObject<Student>(response.Body);
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(Student student)
        {

            client = new FireSharp.FirebaseClient(config);
            SetResponse response = client.Set("Student/" + student.student_id,student);
            return RedirectToAction("Index");
        }
        //delete student
        [HttpGet]
        public ActionResult Delete(String id)
        {

            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Delete("Student/" + id);
            return RedirectToAction("Index");
        }
    }
}