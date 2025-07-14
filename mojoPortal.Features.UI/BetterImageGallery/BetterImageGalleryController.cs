using mojoPortal.Business;
using mojoPortal.Web;
using System.IO;
using System.Web;
using System.Web.Http;

namespace mojoPortal.Features.UI.BetterImageGallery
{
	public class BetterImageGalleryController : ApiController
	{
		// Get list Folders and Images
		[HttpGet]
		[Route("api/BetterImageGallery/")]
		public IHttpActionResult GetItems([FromUri] string path, int moduleId = -1)
		{
			var gallery = new BetterImageGalleryService(moduleId);
			var items = gallery.GetImages(path);

			return Ok(items);
		}

		// Get cached images from systemfiles/BetterImageGallery
		[HttpGet]
		[Route("api/BetterImageGallery/imagehandler")]
		public IHttpActionResult ImageHandler([FromUri] string path)
		{
			// Validate input
			if (string.IsNullOrWhiteSpace(path) || path.IndexOf("..") >= 0)
			{
				return BadRequest("Invalid image path.");
			}
			var baseFolder = HttpContext.Current.Server.MapPath("~/Data/systemfiles/BetterImageGalleryCache/");
			var combinedPath = Path.Combine(baseFolder, path);
			var fullPath = Path.GetFullPath(combinedPath);
			if (!fullPath.StartsWith(baseFolder, StringComparison.OrdinalIgnoreCase))
			{
				return BadRequest("Invalid image path.");
			}
			var fileInfo = new FileInfo(fullPath);

			return !fileInfo.Exists
				? (IHttpActionResult)NotFound()
				: new FileResult(fileInfo.FullName);
		}
	}
}