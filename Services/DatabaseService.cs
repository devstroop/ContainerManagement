using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;

using ContainerManagement.Data;

namespace ContainerManagement
{
    public partial class DatabaseService
    {
        DatabaseContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly DatabaseContext context;
        private readonly NavigationManager navigationManager;

        public DatabaseService(DatabaseContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }


        public async Task ExportNodesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/nodes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/nodes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportNodesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/nodes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/nodes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnNodesRead(ref IQueryable<ContainerManagement.Models.Database.Node> items);

        public async Task<IQueryable<ContainerManagement.Models.Database.Node>> GetNodes(Query query = null)
        {
            var items = Context.Nodes.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnNodesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnNodeGet(ContainerManagement.Models.Database.Node item);
        partial void OnGetNodeByName(ref IQueryable<ContainerManagement.Models.Database.Node> items);


        public async Task<ContainerManagement.Models.Database.Node> GetNodeByName(string name)
        {
            var items = Context.Nodes
                              .AsNoTracking()
                              .Where(i => i.Name == name);

 
            OnGetNodeByName(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnNodeGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnNodeCreated(ContainerManagement.Models.Database.Node item);
        partial void OnAfterNodeCreated(ContainerManagement.Models.Database.Node item);

        public async Task<ContainerManagement.Models.Database.Node> CreateNode(ContainerManagement.Models.Database.Node node)
        {
            OnNodeCreated(node);

            var existingItem = Context.Nodes
                              .Where(i => i.Name == node.Name)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Nodes.Add(node);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(node).State = EntityState.Detached;
                throw;
            }

            OnAfterNodeCreated(node);

            return node;
        }

        public async Task<ContainerManagement.Models.Database.Node> CancelNodeChanges(ContainerManagement.Models.Database.Node item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnNodeUpdated(ContainerManagement.Models.Database.Node item);
        partial void OnAfterNodeUpdated(ContainerManagement.Models.Database.Node item);

        public async Task<ContainerManagement.Models.Database.Node> UpdateNode(string name, ContainerManagement.Models.Database.Node node)
        {
            OnNodeUpdated(node);

            var itemToUpdate = Context.Nodes
                              .Where(i => i.Name == node.Name)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(node);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterNodeUpdated(node);

            return node;
        }

        partial void OnNodeDeleted(ContainerManagement.Models.Database.Node item);
        partial void OnAfterNodeDeleted(ContainerManagement.Models.Database.Node item);

        public async Task<ContainerManagement.Models.Database.Node> DeleteNode(string name)
        {
            var itemToDelete = Context.Nodes
                              .Where(i => i.Name == name)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnNodeDeleted(itemToDelete);


            Context.Nodes.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterNodeDeleted(itemToDelete);

            return itemToDelete;
        }
        }
}