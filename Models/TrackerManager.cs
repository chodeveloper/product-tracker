using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Microsoft.EntityFrameworkCore;
// using MySql.Data.MySqlClient;


namespace productTracker.Models
{
    public class TrackerManager : DbContext
    // public class TrackerManager
    {   
        public DbSet<Instance> instance { get; set; }
        public DbSet<Description> pndesc { get; set; }
        public DbSet<Property> property { get; set; }
        public DbSet<History> history { get; set; }

        public string fPn { get; set; }
        public string fModel { get; set; }
        public string fSn { get; set; }
        public string fParent { get; set; }

        public int numRows { get; set; } = 50;
        public int currentPage { get; set; } = 1;

        public int firstRow;
        public int lastRow;
        public List<Instance> products;
        public int itemTotal;


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseMySQL(Connection.CONNECTION_STRING);
        }

        public int totalPage {
            get {
                return (this.itemTotal/this.numRows) + 1;
            }
        }

        public List<Instance> allProducts {
            get {
                var query = from c in instance join d in pndesc on c.pn equals d.pn into agroup
                        from a in agroup.DefaultIfEmpty()
                        join p in instance on c.pid equals p.id into bgroup
                        from b in bgroup.DefaultIfEmpty()
                        orderby c.id descending
                        select new Instance {
                            id = c.id,
                            pid = c.pid,
                            pn = c.pn,
                            sn = c.sn,
                            model = a.model,
                            ppn = b.pn,
                            psn = b.sn
                        };
                return query.ToList();
            }
        }

        public void setUp() {
            if (this.currentPage == 0) this.currentPage = 1;
            this.firstRow = (this.numRows * this.currentPage) - this.numRows + 1;
            this.lastRow = this.numRows * this.currentPage;
            this.products = getProducts();
        }

        public List<Instance> getProducts() {
            var query = from c in instance join d in pndesc on c.pn equals d.pn into agroup
                        from a in agroup.DefaultIfEmpty()
                        join p in instance on c.pid equals p.id into bgroup
                        from b in bgroup.DefaultIfEmpty()
                        orderby c.id descending
                        select new Instance {
                            id = c.id,
                            pid = c.pid,
                            pn = c.pn,
                            sn = c.sn,
                            model = a.model,
                            ppn = b.pn,
                            psn = b.sn
                        };
            if (!String.IsNullOrEmpty(this.fPn)) query = query.Where(q => q.pn.Contains(this.fPn));
            if (!String.IsNullOrEmpty(this.fModel)) query = query.Where(q => q.model.Contains(this.fModel));
            if (!String.IsNullOrEmpty(this.fSn)) query = query.Where(q => q.sn.Contains(this.fSn));
            if (!String.IsNullOrEmpty(this.fParent)) query = query.Where(q => q.ppn.Contains(this.fParent) || q.psn.Contains(this.fParent));

            this.itemTotal = query.Count();

            var skip = this.numRows * (this.currentPage - 1);
            if (skip < 0) skip = 0;
            var products = query.Skip(skip).Take(this.numRows).ToList();
            return products;
        }

        public Instance getProduct(int selectedId) {

            return getProducts().Find(i=>i.id == selectedId);
        }

        public int getInstanceId(string model) {
            Instance ins = instance.OrderByDescending(i => i.id).ToList().Find(i=>i.pn == model);
            if (ins == null) return 0;
            return instance.OrderByDescending(i => i.id).ToList().Find(i=>i.pn == model).id;
        }

        public List<Property> propList(int id) {
            return property.Where(p=>p.object_id == id).OrderBy(p=> p.name).ToList();
        }

        public List<Instance> childList(int id) {
            var children = instance.Where(i=>i.pid == id).OrderBy(i=>i.id).ToList();
            foreach (Instance child in children) {
                child.properties = property.Where(p=>p.object_id == child.id).OrderBy(p=> p.name).ToList();
            }
            return children;
        }   

        public SelectList SelectParent() {
            var parentList = (from i in instance.ToList()
                             where i.pid == 0
                             orderby i.id descending
                             select new {
                                 id = i.id,
                                 option = i.pn + "(" + i.sn + ")"
                             }).ToList();

            parentList.Insert(0, new { id = 0, option = "No Parent Selected"});

            return new SelectList(parentList, "id", "option");
        }

        public SelectList SelectModel() {
            var modelList = (from i in pndesc.ToList()
                             where i.pn != null
                             orderby i.id descending
                             select new {
                                 id = i.pn,
                                 option = i.pn
                             }).ToList();                          

            modelList.Insert(0, new { id = "", option = "No Model Selected"});

            return new SelectList(modelList, "id", "option");
        }

        public dynamic SelectProperty() {
            var propList = (from p in property.ToList()
                            select new {
                                option = p.name
                            }).Distinct().ToList();

            return propList;
        }

        public List<SelectListItem> SelectPageSize() {
            var pageSize = new List<SelectListItem>(){
                new SelectListItem() {Text="10" , Value="10"},
                new SelectListItem() {Text="30" , Value="30"},
                new SelectListItem() {Text="50" , Value="50"},
                new SelectListItem() {Text="100" , Value="100"}
            };    

            return pageSize;
        }

        public List<SelectListItem> SelectCurrentPage() {
            var pageNum = new List<SelectListItem>();

            for (var i=0; i<this.totalPage; i++) {
                pageNum.Insert(i, (new SelectListItem { Text=(i+1).ToString(), Value=(i+1).ToString() }));

            }

            return pageNum;
        }
    }
}