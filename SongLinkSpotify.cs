        private async Task<string> GetTitleFromSongLink(string search)
        {
            if (!Uri.TryCreate(search, UriKind.Absolute, out var uri) || uri.Scheme != "https")
            {
                return $"Invalid URI: {search}";
            }

            var spotifyId = uri.Segments[2];

            using var response = await Globals.hclient.GetAsync($"https://song.link/s/{spotifyId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return $"Unable to find any results for Song Link: {search}";
            }

            var responseString = await response.Content.ReadAsStringAsync();

            var songName = HttpUtility.HtmlDecode(SongNameSongLinkRegex.Match(responseString).Groups.OfType<Capture>().Skip(1).FirstOrDefault()?.Value);
            if (string.IsNullOrEmpty(songName))
            {
                return $"Couldn't get Song name for {search}";
            }

            return songName;
        }