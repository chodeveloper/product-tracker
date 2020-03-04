using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using productTracker.Models;

namespace productTracker.Controllers {

    public class TrackerController : Controller {

        private TrackerManager tracker;
        private void populateSelect() {
            ViewBag.parentList = tracker.SelectParent();
            ViewBag.modelList = tracker.SelectModel();
            ViewBag.propList = tracker.SelectProperty();
        }

        public TrackerController(TrackerManager myTracker) {
            tracker = myTracker;
        }

        public IActionResult Index() {
            return View(tracker);
        }

        public IActionResult Product() {

            if (TempData["currentPage"] != null) tracker.currentPage = Convert.ToInt32(TempData["currentPage"]);
            if (TempData["numRows"] != null) tracker.numRows = Convert.ToInt32(TempData["numRows"]);

            if (TempData["fPn"] != null) tracker.fPn = TempData["fPn"].ToString();
            if (TempData["fModel"] != null) tracker.fModel = TempData["fModel"].ToString();
            if (TempData["fSn"] != null) tracker.fSn = TempData["fSn"].ToString();
            if (TempData["fParent"] != null) tracker.fParent = TempData["fParent"].ToString();

            Console.WriteLine("\n\n Model: "+tracker.fPn+"\n\n");  
            Console.WriteLine("\n\n Productname: "+tracker.fModel+"\n\n");  
            Console.WriteLine("\n\n Serial: "+tracker.fSn+"\n\n");  
            Console.WriteLine("\n\n Parent: "+tracker.fParent+"\n\n"); 


            tracker.setUp();

            return View(tracker);
        }

        [HttpPost]
        public IActionResult UpdateSelect(int currentPage, int numRows, string fPn, string fModel, string fSn, string fParent) {     

            TempData["currentPage"] = currentPage;
            TempData["numRows"] = numRows;
            TempData["fPn"] = fPn;
            TempData["fModel"] = fModel;
            TempData["fSn"] = fSn;
            TempData["fParent"] = fParent; 

            Console.WriteLine("\n\n\n CurrentPage: "+currentPage+"\n\n\n");
            Console.WriteLine("\n\n\n NumRows: "+numRows+"\n\n\n");

            return RedirectToAction("Product");
        }

        [HttpPost]
        public IActionResult NavigatePage(int p, string nav) {     

            if (nav == "next") TempData["currentPage"] = p + 1;
            else if (nav == "first") TempData["currentPage"] = 1;
            else if (nav == "prev") TempData["currentPage"] = p - 1;
            else if (nav == "last") TempData["currentPage"] = tracker.totalPage;

            Console.WriteLine("\n\n\n CurrentPage: "+TempData["currentPage"]+"\n\n\n");        

            return RedirectToAction("Product");
        }
        
        public IActionResult SearchProduct(string fPn, string fModel, string fSn, string fParent) {
            TempData["fPn"] = fPn;
            TempData["fModel"] = fModel;
            TempData["fSn"] = fSn;
            TempData["fParent"] = fParent; 

            return RedirectToAction("Product");
        }

        public IActionResult ProductAdd(int id) {
            Instance instance;
            if (id != 0) {
                instance = tracker.getProduct(id);
                instance.children = tracker.childList(id);
                instance.properties = tracker.propList(id);
                ViewData["request"] = "Edit";
                populateSelect();
                return View(instance);
            } else {
                instance = new Instance();
                instance.children = new List<Instance>();
                instance.properties = new List<Property>(); 
            }
            ViewData["request"] = "Add";
            populateSelect();
            return View(instance);
        }
        [HttpPost]
        public IActionResult AddModel(Instance instance) { 
            if (instance.newDesc.check == true) instance.newDesc.active = "TRUE";
            else instance.newDesc.active = "FALSE";
            // Description desc = new Description();
            // desc = instance.newDesc;
            if (instance.newDesc.pn==null || instance.newDesc.model==null || instance.newDesc.description==null) {
                ViewData["error"] = "Error: All text fields for new model data are required - Failed to add new model :(";
            } else {
                tracker.Add(instance.newDesc);
                tracker.SaveChanges();
                instance.newDesc = new Description();
                ViewData["error"] = "";
            }
            populateSelect();
            return View("ProductAdd", instance);
        }
        [HttpPost]
        public IActionResult SelectModel(Instance instance) {         
            int id = tracker.getInstanceId(instance.pn);  
            if (id != 0) instance.properties = tracker.propList(id).ToList();            
            populateSelect();
            return View("ProductAdd", instance);
        }
        [HttpPost]
        public IActionResult AddProperty(Instance instance) {
            Property property = new Property();
            instance.properties.Add(property);
            populateSelect();
            return View("ProductAdd", instance);
        }
        [HttpPost]
        public IActionResult DeleteProperty(Instance instance, int prop) {
            instance.properties.RemoveAt(prop); 
            populateSelect();
            return View("ProductAdd", instance);
        }
        [HttpPost]
        public IActionResult AddChild(Instance instance) {
            Instance child = new Instance();
            instance.children.Add(child);
            populateSelect();
            ViewBag.selected = instance.children.Count - 1;
            return View("ProductAdd", instance);
        }
        [HttpPost]
        public IActionResult DeleteChild(Instance instance, int ch) {
            instance.children.RemoveAt(ch); 
            populateSelect();
            return View("ProductAdd", instance);
        }
        [HttpPost]
        public IActionResult SelectChildModel(Instance instance, int ch) {         
            int id = tracker.getInstanceId(instance.children[ch].pn);  
            if (id != 0) instance.children[ch].properties = tracker.propList(id).ToList();            
            populateSelect();
            ViewBag.selected = ch;
            return View("ProductAdd", instance);
        }
        [HttpPost]
        public IActionResult AddChildProperty(Instance instance, int ch) {
            Property property = new Property();
            instance.children[ch].properties.Add(property);
            populateSelect();
            ViewBag.selected = ch;
            return View("ProductAdd", instance);
        }
        [HttpPost]
        public IActionResult DeleteChildProperty(Instance instance, int ch, int prop) {
            instance.children[ch].properties.RemoveAt(prop); 
            populateSelect();
            ViewBag.selected = ch;
            return View("ProductAdd", instance);
        }
        [HttpPost]
        public IActionResult ProductAddSubmit(Instance instance) {
            populateSelect();

            if (!ModelState.IsValid) {
                ViewData["error"] = "Error: All text fields and model# are required - Failed to add new product(s) :(";
                return View("ProductAdd", instance);
            }
            ViewData["error"] = "";
            tracker.Add(instance);
            tracker.SaveChanges();
            
            History build = new History();
            build.entrytime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            build.eventdate = DateTime.Now.ToString("yyyy-MM-dd");
            build.eventdesc = "BUILD";
            build.instid = instance.id;
            tracker.Add(build);
            tracker.SaveChanges();
            
            if (instance.properties.Count > 0) {
                instance.properties.ForEach(p => {
                    p.object_type = 3;
                    p.object_id = instance.id;
                    tracker.Add(p);
                    tracker.SaveChanges();
                });
            }

            if (instance.children.Count > 0) {
                instance.children.ForEach(c => {
                    c.pid = instance.id;
                    tracker.Add(c);
                    tracker.SaveChanges();
                    History childBuild = new History();
                    childBuild.entrytime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                    childBuild.eventdate = DateTime.Now.ToString("yyyy-MM-dd");
                    childBuild.eventdesc = "BUILD";
                    childBuild.instid = c.id;
                    tracker.Add(childBuild);
                    tracker.SaveChanges();

                    if (c.properties.Count > 0) {
                        c.properties.ForEach(cp => {
                            cp.object_type = 3;
                            cp.object_id = c.id;
                            tracker.Add(cp);
                            tracker.SaveChanges();
                        });
                    }
                });
            }
            return RedirectToAction("Product");
        }

        [HttpPost]
        public IActionResult ProductEditSubmit(Instance instance) {
            populateSelect();

            if (!ModelState.IsValid) {
                ViewData["error"] = "Error: All text fields and model# are required - Failed to add new product(s) :(";
                return View("ProductAdd", instance);
            }
            ViewData["error"] = "";

            tracker.Update(instance);
            tracker.SaveChanges();
            
            if (instance.properties.Count > 0) {
                instance.properties.ForEach(p => {
                    tracker.Update(p);
                    tracker.SaveChanges();
                });
            }

            if (instance.children.Count > 0) {
                instance.children.ForEach(c => {
                    tracker.Update(c);
                    tracker.SaveChanges();

                    if (c.properties.Count > 0) {
                        c.properties.ForEach(cp => {
                            tracker.Update(cp);
                            tracker.SaveChanges();
                        });
                    }
                });
            }
            return RedirectToAction("Product");
        }




        public IActionResult Description(TrackerManager tracker)
        {
            return View(tracker);
        }

        // public IActionResult Event(TrackerManager tracker)
        // {
        //     return View(tracker);
        // }

        // public IActionResult Contact(TrackerManager tracker)
        // {
        //     return View(tracker);
        // }
    }
}
