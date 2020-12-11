using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartTV
{
	/// <summary>
	/// YouTube Data API v3 sample: search by keyword.
	/// Relies on the Google APIs Client Library for .NET, v1.7.0 or higher.
	/// See https://developers.google.com/api-client-library/dotnet/get_started
	///
	/// Set ApiKey to the API key value from the APIs & auth > Registered apps tab of
	///   https://cloud.google.com/console
	/// Please ensure that you have enabled the YouTube Data API for your project.
	/// </summary>
	internal class Search
	{
		[STAThread]

		public async Task<List<Video>> Run(string keyword)
		{
			var youtubeService = new YouTubeService(new BaseClientService.Initializer()
			{
				ApiKey = "AIzaSyBUp9lbDZdmeve021XVdDFgUg6a6zXS2WI",
				ApplicationName = this.GetType().ToString()
			});

			var searchListRequest = youtubeService.Search.List("snippet");
			searchListRequest.Q = keyword; // Replace with your search term.
			searchListRequest.MaxResults = 50;

			// Call the search.list method to retrieve results matching the specified query term.
			var searchListResponse = await searchListRequest.ExecuteAsync();

			List<Video> videos = new List<Video>();
			List<string> channels = new List<string>();
			List<string> playlists = new List<string>();

			// Add each result to the appropriate list, and then display the lists of
			// matching videos, channels, and playlists.
			foreach (var searchResult in searchListResponse.Items)
			{
				switch (searchResult.Id.Kind)
				{
					case "youtube#video":
						Video v = new Video();
						v.Title = searchResult.Snippet.Title;
						v.Chanel = searchResult.Snippet.ChannelTitle;
						byte[] im = new WebClient().DownloadData(searchResult.Snippet.Thumbnails.Default__.Url);
						using (MemoryStream ms = new MemoryStream(im))
						{
							v.Thumbnail = Image.FromStream(ms);
						}
						v.Link = "https://www.youtube.com/watch?v=" + searchResult.Id.VideoId;
						videos.Add(v);
						break;

					case "youtube#channel":
						channels.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.ChannelId));
						break;

					case "youtube#playlist":
						playlists.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.PlaylistId));
						break;
				}
			}

			return videos;
		}
	}
}
