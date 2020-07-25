using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetMongoIDTest.Models;
using AspNetMongoIDTest.Services;
using Microsoft.AspNetCore.Authorization;

namespace AspNetMongoIDTest.Controllers
{
    public class ExploreDbController : Controller
    {
        private readonly DocumentService _documentService;

        public ExploreDbController(DocumentService documentService)
        {
            _documentService = documentService;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string selectedDatabase, string selectedCollection, int index = 0)
        {
            System.Diagnostics.Debug.WriteLine("EXECUTING!");
            var databasesAndCollections = await _documentService.GetDatabasesAndCollections();
            System.Diagnostics.Debug.WriteLine(databasesAndCollections.Count);
            var viewModel = new ExplorerDbViewModel()
            {
                DatabasesAndCollections = databasesAndCollections,
                Database = selectedDatabase,
                Collection = selectedCollection,
                Index = index
            };
            if (selectedCollection != null && selectedDatabase != null)
            {
                viewModel.Document = await _documentService.GetDocument(selectedDatabase, selectedCollection, index);
                viewModel.CollectionCount = await _documentService.GetCollectionCount(selectedDatabase, selectedCollection);
            }
            return View(viewModel);
        }

        [Authorize(Roles = "Everyone")]
        public async Task<IActionResult> Everyone(string selectedDatabase, string selectedCollection, int index = 0)
        {
            var databasesAndCollections = await _documentService.GetDatabasesAndCollections();
            var viewModel = new ExplorerDbViewModel()
            {
                DatabasesAndCollections = databasesAndCollections,
                Database = selectedDatabase,
                Collection = selectedCollection,
                Index = index
            };
            if (selectedCollection != null && selectedDatabase != null)
            {
                viewModel.Document = await _documentService.GetDocument(selectedDatabase, selectedCollection, index);
                viewModel.CollectionCount = await _documentService.GetCollectionCount(selectedDatabase, selectedCollection);
            }
            return View(viewModel);
        }

        public async Task<IActionResult> CreateOrUpdate(
          string database,
          string collection,
          string id,
          int index,
          string fieldName,
          string value
        )
        {
            await _documentService.CreateOrUpdateField(database, collection, id, fieldName, value);
            return RedirectToAction("Index", GetRouteValues(database, collection, index));
        }

        public async Task<IActionResult> CreateDoc(
          string database,
          string collection
        )
        {
            await _documentService.CreateDocument(database, collection);
            var count = await _documentService.GetCollectionCount(database, collection);
            return RedirectToAction("Index", GetRouteValues(database, collection, count - 1));
        }

        public async Task<IActionResult> DeleteDoc(
          string database,
          string collection,
          string id,
          int index
        )
        {
            var delete = await _documentService.DeleteDocument(database, collection, id);
            return RedirectToAction("Index", GetRouteValues(database, collection, 0));
        }

        public async Task<IActionResult> CreateCol(
          string database,
          string value
        )
        {
            await _documentService.NewCollection(database, value);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteCol(
          string database,
          string value
        )
        {
            await _documentService.DeleteCollection(database, value);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> CreateDb(
          string value
        )
        {
            await _documentService.NewDatabase(value);
            //await Index(null, null, 0);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteDb(
          string value
        )
        {
            await _documentService.DeleteDatabase(value);
            return RedirectToAction("Index");
        }

        private static object GetRouteValues(string database, string collection, long index)
        {
            return new { selectedDatabase = database, selectedCollection = collection, index = index };
        }
    }
}
