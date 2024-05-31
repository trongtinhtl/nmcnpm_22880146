using HtmlAgilityPack;
using ITConferences.Enums;
using ITConferences.Managers;
using ITConferences.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Reflection.Metadata;
using System.Text;

namespace ITConferences.Helpers
{
    public class ITConferecesCrawler
	{
		public static List<ITConferenceModel> CrawlDevEvents(CrawlerModel crawler, int page, out int totalCount, ref List<CountryModel> Countries, ref List<TopicModel> Topics)
		{
			totalCount = 0;
			var results = new List<ITConferenceModel>();

			if (crawler != null && crawler.id > 0 && !string.IsNullOrEmpty(crawler.crawlerUrl))
			{
				Countries = Countries ?? new List<CountryModel>();
				Topics = Topics ?? new List<TopicModel>();

				using (HttpClient httpClient = new HttpClient())
				{
					var getData = httpClient.GetStringAsync($"{crawler.crawlerUrl}/?page={page}");

					if (!string.IsNullOrEmpty(getData?.Result))
					{
						HtmlDocument htmlDocument = new HtmlDocument();
						htmlDocument.LoadHtml(getData.Result);

						HtmlNode parentElement = htmlDocument.GetElementbyId("events");

						if (parentElement != null)
						{
							var divElements = parentElement.Descendants("div");

							foreach (var div in divElements)
							{
								if (div != null)
								{
									var scriptElements = div.Descendants("script");

									var navElements = div.Descendants("nav");

									if (scriptElements?.Count() > 0)
									{
										var script = scriptElements.First();

										if (!string.IsNullOrEmpty(script?.InnerHtml))
										{
											ITConferenceModel? vmData = null;

											try
											{
												JObject jsonData = JObject.Parse(script.InnerHtml);

												if (jsonData != null)
												{
													vmData = new ITConferenceModel();

													vmData.crawlerId = crawler.id;
													var date = DateTime.Now;
													vmData.crawlDate = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);

													if (jsonData.ContainsKey("name"))
													{
														vmData.conferenceName = jsonData["name"]?.ToString();
													}

													if (jsonData.ContainsKey("description"))
													{
														vmData.description = jsonData["description"]?.ToString();
													}

													if (jsonData.ContainsKey("@type"))
													{
														vmData.type = jsonData["@type"]?.ToString();
													}

													if (jsonData.ContainsKey("startDate") && DateTime.TryParse(jsonData["startDate"]?.ToString(), out DateTime startDate))
													{
														vmData.startDate = startDate;
													}

													if (jsonData.ContainsKey("endDate") && DateTime.TryParse(jsonData["endDate"]?.ToString(), out DateTime endDate))
													{
														vmData.endDate = endDate;
													}

													if (jsonData.ContainsKey("url"))
													{
														vmData.link = jsonData["url"]?.ToString();
													}

													if (jsonData.ContainsKey("image"))
													{
														string? image = jsonData["image"]?.ToString();

														if (!string.IsNullOrEmpty(image))
														{
															vmData.images = JsonConvert.DeserializeObject<List<string>>(image);
														}
													}

													if (jsonData.ContainsKey("performer") && jsonData["performer"] != null)
													{
														vmData.performer = jsonData["performer"]["name"]?.ToString();
													}

													if (jsonData.ContainsKey("location") && jsonData["location"] != null)
													{
														vmData.location = jsonData["location"]["name"]?.ToString();

														if (jsonData["location"]["address"] != null)
														{
															var countryName = GetString(jsonData["location"]["address"]["addressRegion"]?.ToString());

															if (!string.IsNullOrEmpty(countryName))
															{
																var country = Countries.Find(t => t.countryName == countryName);

																if (country != null)
																{
																	vmData.countryId = country.id;
																}
																else
																{
																	int id = 1;
																	var last = Countries.OrderBy(t => t.id).LastOrDefault();
																	if (last != null) id = last.id + 1;

																	country = new CountryModel(id, countryName);
																	vmData.countryId = id;
																	Countries.Add(country);
																}
															}
														}
													}
												}

												if (vmData != null)
												{
													var subTitles = div.Descendants("h3")
														.Where(t => t.Attributes.Contains("class") && t.Attributes["class"].Value.Contains("subtitle"));

													if (subTitles?.Count() > 0)
													{
														var subTitle = subTitles.FirstOrDefault();

														if (subTitle != null)
														{
															var links = subTitle.Descendants('a').Where(t => t.Attributes.Contains("href"));

															if (links?.Count() > 0)
															{
																var linkTech = links.FirstOrDefault();

																if (linkTech != null)
																{
																	var topicName = GetString(linkTech.InnerText);

																	if (!string.IsNullOrEmpty(topicName))
																	{
																		var topic = Topics.Find(t => t.topicName == topicName);

																		if (topic != null)
																		{
																			vmData.topicId = topic.id;
																		}
																		else
																		{
																			int id = 1;
																			var last = Topics.OrderBy(t => t.id).LastOrDefault();
																			if (last != null) id = last.id + 1;

																			topic = new TopicModel(id, topicName);
																			vmData.topicId = id;
																			Topics.Add(topic);
																		}
																	}
																}
															}
														}
													}
												}
											}
											catch (Exception)
											{
												vmData = null;
											}

											if (vmData != null)
											{
												results.Add(vmData);
											}
										}
									}

									if (navElements?.Count() > 0)
									{
										var nav = navElements.First();

										if (!string.IsNullOrEmpty(nav?.InnerText))
										{
											var splits = nav.InnerText.Split(" ");

											foreach (var item in splits)
											{
												if (int.TryParse(item, out int number))
												{
													if (number > totalCount) totalCount = number;
												}
											}
										}
									}
								}
							}
						}
						else
						{
							Console.WriteLine("Parent element not found!");
						}
					}
				}
			}

			return results;
		}

        public static List<ITConferenceModel> CrawlEventyco(CrawlerModel crawler, int page, out int totalPage, ref List<CountryModel> Countries, ref List<TopicModel> Topics)
        {
            totalPage = 0;
            var results = new List<ITConferenceModel>();

            if (crawler != null && crawler.id > 0 && !string.IsNullOrEmpty(crawler.crawlerUrl))
            {
                Countries = Countries ?? new List<CountryModel>();
                Topics = Topics ?? new List<TopicModel>();

                using (HttpClient httpClient = new HttpClient())
                {
                    var getData = httpClient.GetStringAsync($"{crawler.crawlerUrl}~{page}");

                    if (!string.IsNullOrEmpty(getData?.Result))
                    {
                        HtmlDocument htmlDocument = new HtmlDocument();
                        htmlDocument.LoadHtml(getData.Result);

                        HtmlNode parentElement = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='row row-cols-1 row-cols-sm-2 row-cols-lg-3']");

                        if (parentElement != null)
                        {
                            var pageElement = htmlDocument.DocumentNode.SelectSingleNode("//ul[@class='pagination justify-content-center']");

                            var divElements = parentElement.SelectNodes("//div[@class='col mb-3 mb-lg-5']");

                            if (divElements.Any())
                            {
                                foreach (var div in divElements)
                                {
                                    if (div != null)
                                    {
                                        var scriptElements = div.Descendants("script");

                                        if (scriptElements?.Count() > 0)
                                        {
                                            var script = scriptElements.First();

                                            if (!string.IsNullOrEmpty(script?.InnerHtml))
                                            {
                                                ITConferenceModel? vmData = null;

                                                try
                                                {

                                                    JObject jsonData = JObject.Parse(script.InnerHtml);

                                                    if (jsonData != null)
                                                    {
                                                        vmData = new ITConferenceModel();

                                                        vmData.crawlerId = crawler.id;
                                                        var date = DateTime.Now;
                                                        vmData.crawlDate = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);

                                                        if (jsonData.ContainsKey("name"))
                                                        {
                                                            vmData.conferenceName = GetString(jsonData["name"]?.ToString());
                                                        }

                                                        if (jsonData.ContainsKey("description"))
                                                        {
                                                            vmData.description = GetString(jsonData["description"]?.ToString());
                                                        }

                                                        if (jsonData.ContainsKey("@type"))
                                                        {
                                                            vmData.type = jsonData["@type"]?.ToString();
                                                        }

                                                        if (jsonData.ContainsKey("startDate") && DateTime.TryParse(jsonData["startDate"]?.ToString(), out DateTime startDate))
                                                        {
                                                            vmData.startDate = startDate;
                                                        }

                                                        if (jsonData.ContainsKey("endDate") && DateTime.TryParse(jsonData["endDate"]?.ToString(), out DateTime endDate))
                                                        {
                                                            vmData.endDate = endDate;
                                                        }

                                                        if (jsonData.ContainsKey("performer") && jsonData["performer"] != null)
                                                        {
                                                            vmData.performer = jsonData["performer"]["name"]?.ToString();
                                                        }

                                                        if (jsonData.ContainsKey("location") && jsonData["location"] != null && jsonData["location"]["address"] != null)
                                                        {
                                                            var countryName = GetString(jsonData["location"]["address"]["addressCountry"]?.ToString());
                                                            vmData.location = GetString(jsonData["location"]["address"]["addressLocality"]?.ToString());

                                                            if (!string.IsNullOrEmpty(countryName))
                                                            {
                                                                vmData.location += string.IsNullOrEmpty(vmData.location) ? countryName : ", " + countryName;

                                                                var country = Countries.Find(t => t.countryName == countryName);

                                                                if (country != null)
                                                                {
                                                                    vmData.countryId = country.id;
                                                                }
                                                                else
                                                                {
                                                                    int id = 1;
                                                                    var last = Countries.OrderBy(t => t.id).LastOrDefault();
                                                                    if (last != null) id = last.id + 1;

                                                                    country = new CountryModel(id, countryName);
                                                                    vmData.countryId = id;
                                                                    Countries.Add(country);
                                                                }
                                                            }
                                                        }

                                                        var titleElement = div.SelectSingleNode("//h3[@class='card-title text-truncate']");

                                                        if (titleElement != null)
                                                        {
                                                            var node = titleElement.Descendants("a")?.FirstOrDefault();

                                                            if (node != null)
                                                            {
                                                                string hrefValue = node.GetAttributeValue("href", "");
                                                                if (!string.IsNullOrEmpty(hrefValue))
                                                                {
                                                                    vmData.link = hrefValue;
                                                                    if (hrefValue.Contains("http") == false)
                                                                        vmData.link = "https://www.eventyco.com" + hrefValue;
                                                                }
                                                            }
                                                        }

                                                        var tagElement = div.SelectSingleNode("//a[@class='btn btn-soft-primary btn-xs mt-1']");

                                                        if (tagElement != null)
                                                        {
                                                            var topicName = GetString(tagElement.InnerText);

                                                            if (!string.IsNullOrEmpty(topicName))
                                                            {
                                                                var topic = Topics.Find(t => t.topicName == topicName);

                                                                if (topic != null)
                                                                {
                                                                    vmData.topicId = topic.id;
                                                                }
                                                                else
                                                                {
                                                                    int id = 1;
                                                                    var last = Topics.OrderBy(t => t.id).LastOrDefault();
                                                                    if (last != null) id = last.id + 1;

                                                                    topic = new TopicModel(id, topicName);
                                                                    vmData.topicId = id;
                                                                    Topics.Add(topic);
                                                                }
                                                            }
                                                        }
                                                    }

                                                }
                                                catch (Exception)
                                                {
                                                    vmData = null;
                                                }

                                                if (vmData != null)
                                                {
                                                    results.Add(vmData);
                                                }
                                            }
                                        }

                                    }
                                }

                            }

                            if (pageElement != null)
                            {
                                var pageNexts = parentElement.SelectNodes("//li[@class='page-item next']");

                                if (pageNexts.Count > 0)
                                {
                                    var last = pageNexts.LastOrDefault();

                                    if (last != null)
                                    {
                                        var node = last.Descendants("a")?.FirstOrDefault();
                                        if (node != null)
                                        {
                                            string hrefValue = node.GetAttributeValue("href", "");

                                            if (!string.IsNullOrEmpty(hrefValue) && hrefValue.Contains("~"))
                                            {
                                                var arr = hrefValue.Split("~");
                                                if (arr.Length == 2 && int.TryParse(arr[1], out int number))
                                                {
                                                    totalPage = number;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return results;
        }

        public static List<ITConferenceModel> CrawlPolytechnique(CrawlerModel crawler, ref List<CountryModel> Countries, ref List<TopicModel> Topics)
		{
			var results = new List<ITConferenceModel>();

			if (crawler != null && crawler.id > 0 && !string.IsNullOrEmpty(crawler.crawlerUrl))
			{
				Countries = Countries ?? new List<CountryModel>();
				Topics = Topics ?? new List<TopicModel>();

				using (HttpClient httpClient = new HttpClient())
				{
					var getData = httpClient.GetStringAsync(crawler.crawlerUrl);

					string format = "d MMMM yyyy";

					if (!string.IsNullOrEmpty(getData?.Result))
					{
						var reponse = getData.Result;

						reponse = DecodeUnicodeCharacters(reponse);

						HtmlDocument htmlDocument = new HtmlDocument();
						htmlDocument.LoadHtml(getData.Result);

						var tableElements = htmlDocument.DocumentNode.Descendants("table")
												.Where(t => t.Attributes.Contains("class") && t.Attributes["class"].Value.Contains("conference"));

						if (tableElements?.Count() > 0)
						{
							foreach (var table in tableElements)
							{
								var tbody = table.Descendants("tbody")?.FirstOrDefault();

								if (tbody != null)
								{
									var rows = tbody.Descendants("tr");

									if (rows?.Count() > 0)
									{
										foreach (var row in rows)
										{
											ITConferenceModel? vmData = null;

											var columns = row.Descendants("td");

											if (columns?.Count() > 0)
											{
												try
												{
													vmData = new ITConferenceModel();
													vmData.crawlerId = crawler.id;
													var dateNow = DateTime.Now;
													vmData.crawlDate = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, dateNow.Hour, dateNow.Minute, dateNow.Second);

													foreach (var col in columns)
													{
														if (col.HasClass("confname"))
														{
															var link = col.Descendants("a")?.FirstOrDefault();
															var span = col.Descendants("span")?.FirstOrDefault();

															if (link != null)
															{
																vmData.link = link.GetAttributeValue("href", "");
																vmData.conferenceName = GetString(link.InnerText);
															}

															if (span != null)
															{
																vmData.description = GetString(span.InnerText);
															}
														}
														else if (col.HasClass("location"))
														{
															if (!string.IsNullOrEmpty(col.InnerText))
															{
																vmData.location = GetString(col.InnerText);
																var slipts = col.InnerText.Split(',');
																if (slipts.Length > 1)
																{
																	var countryName = GetString(slipts.LastOrDefault());

																	if (!string.IsNullOrEmpty(countryName))
																	{
																		var country = Countries.Find(t => t.countryName == countryName);

																		if (country != null)
																		{
																			vmData.countryId = country.id;
																		}
																		else
																		{
																			int id = 1;
																			var last = Countries.OrderBy(t => t.id).LastOrDefault();
																			if (last != null) id = last.id + 1;

																			country = new CountryModel(id, countryName);
																			vmData.countryId = id;
																			Countries.Add(country);
																		}
																	}
																}
															}
														}
														else if (col.HasClass("date"))
														{
															var value = GetString(col.InnerText);
															var hasBreak = col.Descendants("br")?.Count() > 0;

															if (hasBreak)
															{
																value = string.Empty;
																var splits = col.InnerHtml.Split("<br>");

																if (splits.Length > 1)
																{
																	value = splits.LastOrDefault()?.Trim();
																}
															}

															if (!string.IsNullOrEmpty(value))
															{
																string[]? splits = new string[] { };
																if (value.Contains("-"))
																{
																	splits = value.Split("-");
																}
																else if (value.Contains("–"))
																{
																	splits = value.Split("–");
																}

																if (splits?.Length == 2)
																{
																	var start = GetString(splits[0]);
																	var end = GetString(splits[1]);

																	var endSplits = end?.Split(" ") ?? new string[0];

																	if (endSplits.Length >= 3)
																	{
																		var month = endSplits[1].Trim();
																		var year = endSplits[2].Trim();

																		var startSplits = start?.Split(" ") ?? new string[0];

																		if (startSplits.Length == 2)
																		{
																			start += $" {year}";
																		}
																		else if (startSplits.Length == 1)
																		{
																			start += $" {month} {year}";
																		}

																		if (DateTime.TryParseExact(start, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate))
																		{
																			vmData.startDate = startDate;
																		}

																		if (DateTime.TryParseExact(end, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
																		{
																			vmData.endDate = endDate;
																		}
																	}
																}
																else
																{
																	if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
																	{
																		vmData.startDate = date;
																		vmData.endDate = date;
																	}
																}
															}
														}
														else if (col.HasClass("starting-date"))
														{
															if (DateTime.TryParseExact(col.InnerText, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate))
															{
																vmData.startDate = startDate;
															}
														}
														else if (col.HasClass("ending-date"))
														{
															if (DateTime.TryParseExact(col.InnerText, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
															{
																vmData.endDate = endDate;
															}
														}
													}
												}
												catch (Exception)
												{
													vmData = null;
												}
											}

											if (vmData != null)
											{
												results.Add(vmData);
											}
										}
									}
								}
							}
						}
					}
				}
			}

			return results;
		}

		private static string? GetString(string? value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				value = value.Replace(@"[\r\n]+", "").Replace("&nbsp;", " ");
				return value.Trim();
			}

			return value;
		}

		private static string DecodeUnicodeCharacters(string input)
		{
			return Encoding.UTF8.GetString(Encoding.GetEncoding("ISO-8859-1").GetBytes(input));
		}
	}
}
