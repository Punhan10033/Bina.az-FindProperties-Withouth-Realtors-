using ConsoleApp1;
using DocumentFormat.OpenXml.Drawing.Charts;
using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using OpenQA.Selenium.Support.UI;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Net;
using PuppeteerSharp;
using OpenQA.Selenium.Firefox;
using System.Xml;
using OpenQA.Selenium.DevTools.V112.Network;

namespace Wdwadwa
{



    public class Program
    {
        static void Main()
        {
            Console.WriteLine("Start ? y/n");
            do
            {
                //string s = Console.ReadLine();
                //if (s == "y")
                //{
                //Console.Clear();
                //GetData2();
                GetData();
                //Console.WriteLine("Wanna stop loop ? y=continue, n=stop");
                //}

                //if (s == "n")
                //{
                //    break;
                //}

            }
            while (true);
        }
        //static void GetData2()
        //{
        //    Console.WriteLine("---------------------------------------------------------------------------------------------------------------------");
        //    var baseurl = "https://bina.az/alqi-satqi/evler";
        //    var web = new HtmlWeb();
        //    int pagenumber = 1;
        //    while (true)
        //    {
        //        pagenumber++;
        //        if (pagenumber > 20)
        //        {
        //            break;
        //        }
        //        var url = $"{baseurl}?page={pagenumber}";
        //        var doc = web.Load(url);
        //        var nodes = doc.DocumentNode.SelectNodes("/html/body/table/tbody/tr[2]/td/table/tbody/tr[3]/td/table/tbody/tr[1]/td[position()<10]");
        //        if (nodes != null)
        //        {
        //            foreach (var node in nodes)
        //            {
        //                var itemlinkcheck = node.SelectSingleNode(".//a");
        //                if (!node.InnerHtml.Contains("#") && itemlinkcheck != null)
        //                {
        //                    var url2 = node.SelectSingleNode(".//a").GetAttributeValue("href", "");
        //                    var itemlinknode = node.SelectSingleNode(".//a");
        //                    if (itemlinknode != null)
        //                    {
        //                        var itemlink = "https://tap.az" + node.SelectSingleNode(".//a").GetAttributeValue("href", " ");
        //                        var modifiedurl = itemlink.Replace("/bookmark", "");
        //                        Console.WriteLine("\n" + modifiedurl);
        //                        Console.WriteLine("Price: " + node.SelectSingleNode(".//a/div[2]").InnerText);
        //                        Console.WriteLine("Price: " + node.SelectSingleNode(".//a/div[3]").InnerText);
        //                    }
        //                }

        //            }
        //        }
        //    }
        //}




        static void GetData()
        {
            List<Emlak> emlaklar = new List<Emlak>();
            Console.WriteLine("---------------------------------------------------------------------------------------------------------------------");
            var baseUrl = "https://bina.az/alqi-satqi/menziller";
            var web = new HtmlWeb();
            //
            string con = "Server=localhost;Database=ForConsoleAppEstate;Trusted_Connection=True;MultipleActiveResultSets=true";
            SqlConnection consql = new SqlConnection(con);
            consql.Open();
            using (SqlCommand command = new SqlCommand("Select * from Emlaklar", consql))
            {
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    // Iterate over the rows and populate the list with Emlak objects
                    while (reader.Read())
                    {
                        Emlak emlak = new Emlak();
                        emlak.EmlakId = reader.GetInt32(reader.GetOrdinal("EmlakId"));
                        emlak.ElanId = reader.GetInt32(reader.GetOrdinal("ElanId")).ToString();
                        emlak.ElanUrl = reader.GetString(reader.GetOrdinal("ElanUrl"));
                        emlak.Price = reader.GetString(reader.GetOrdinal("Price"));
                        emlak.Location = reader.GetString(reader.GetOrdinal("Location"));
                        emlak.Otaqlar = reader.GetString(reader.GetOrdinal("Otaqlar"));
                        emlak.Kvadrat = reader.GetString(reader.GetOrdinal("Kvadrat"));
                        emlak.Date = reader.GetString(reader.GetOrdinal("Date"));
                        emlaklar.Add(emlak);
                    }
                }
            }
            //
            Console.WriteLine("Connection is Open");

            int pageNumber = 0;
            int pathnumber = 0;


            while (true)
            {

                //1735
                if (pageNumber > 200)
                {
                    pageNumber = 0;
                }

                var url = $"{baseUrl}?page={pageNumber}";
                var doc = web.Load(url);
                var nodes = doc.DocumentNode.SelectNodes("//*[@id=\"js-items-search\"]/div[position()>1]/div[position()<20]");
                if (nodes != null)
                {

                    foreach (var node in nodes)
                    {
                        var url2 = node.SelectSingleNode(".//a");
                        int lastindx = 0;
                        if (url2 != null)
                        {
                            lastindx = url2.GetAttributeValue("href", "").LastIndexOf("/");
                        }

                        var divInnerText = node.InnerText.Trim();
                        if (!node.InnerText.Contains("Agentlik") && !node.InnerText.Contains("sot"))
                        {
                            var nd = node.SelectSingleNode("//*[@id=\"js-main-col\"]/div[3]/div[17]/div[3]/div[3]/div");
                            var itemlinknode = node.SelectSingleNode(".//a");
                            if (itemlinknode != null)
                            {

                                var itemLink = "https://bina.az" + node.SelectSingleNode(".//a").GetAttributeValue("href", "");
                                var itemInfo = GetIsVasiteci(itemLink);
                                if (!itemInfo && itemLink != "https://bina.az?items_view=map" && !itemLink.Contains("https://bina.az/yasayis-kompleksleri"))
                                {
                                    var nodetocheck = node.SelectSingleNode("div[3]");
                                    doc = web.Load(itemLink);
                                    var buildingtype = doc.DocumentNode.SelectSingleNode("//*[@id=\"js-search-results\"]/div[4]/div[2]/div/main/section[3]/div/div/div[1]/span");
                                    if (nodetocheck != null)
                                    {

                                        var nodddee = node.SelectSingleNode("div[3]/div");
                                        var lstindex = nodddee.InnerText.LastIndexOf("A");
                                        var endindex = nodddee.InnerText.LastIndexOf("N");
                                        var price = double.Parse(nodddee.InnerText.Remove(lstindex).Replace(" ", ""));
                                        var views = int.Parse(doc.DocumentNode.SelectSingleNode("//*[@id=\"js-search-results\"]/div[4]/div[2]/div/main/section[2]/div/div[2]/span").InnerText.Remove(0, doc.DocumentNode.SelectSingleNode("//*[@id=\"js-search-results\"]/div[4]/div[2]/div/main/section[2]/div/div[2]/span").InnerText.LastIndexOf(" ")));
                                        var kvadrat = doc.DocumentNode.SelectSingleNode("//*[@id=\"js-search-results\"]/div[4]/div[2]/div/main/section[3]/div/div/div[3]/span");
                                        if (views < 400)
                                        {

                                            if (price > 0 && price < 10000000)
                                            {

                                                Console.WriteLine("\n Page: " + pageNumber);

                                                Console.WriteLine("                    \nhttps://bina.az" + node.SelectSingleNode(".//a").GetAttributeValue("href", "") + " " + doc.DocumentNode.SelectSingleNode("//*[@id=\"js-search-results\"]/div[4]/div[2]/div/main/section[2]/div/div[2]/span").InnerText);
                                                var link = "https://bina.az" + node.SelectSingleNode(".//a").GetAttributeValue("href", "");
                                                Console.WriteLine("                    Location: " + node.SelectSingleNode("div[3]/div[2]").InnerText);
                                                if (buildingtype != null)
                                                {
                                                    Console.WriteLine("                    Type: " + buildingtype.InnerText);
                                                }
                                                Console.WriteLine("                    Sened: " + doc.DocumentNode.SelectSingleNode("//*[@id=\"js-search-results\"]/div[4]/div[2]/div/main/section[3]/div/div/div[5]/span").InnerText);
                                                var floor = doc.DocumentNode.SelectSingleNode("//*[@id=\"js-search-results\"]/div[4]/div[2]/div/main/section[3]/div/div/div[2]/span");


                                                if (floor != null)
                                                {
                                                    Console.WriteLine("                    Floor: "+floor.InnerText);
                                                }
                                                var date = node.SelectSingleNode("div[3]/div[3]").InnerText;
                                                var indexdate = date.IndexOf(" ");
                                                var elaniddivtext = doc.DocumentNode.SelectSingleNode("//*[@id=\"js-search-results\"]/div[4]/div[2]/div/main/section[6]/div/div[2]");
                                                string elanidnumber = "";
                                                if (elaniddivtext != null)
                                                {
                                                     elanidnumber = elaniddivtext.InnerText.Substring(elaniddivtext.InnerText.IndexOf(":") + 1).Trim();
                                                }

                                                Emlak emlak = new Emlak()
                                                {
                                                    ElanUrl = link,
                                                    PapperWork = doc.DocumentNode.SelectSingleNode("//*[@id=\"js-search-results\"]/div[4]/div[2]/div/main/section[3]/div/div/div[5]/span").InnerText,
                                                    Type = buildingtype.InnerText,
                                                    Location = node.SelectSingleNode("div[3] / div[2]").InnerText,
                                                    Price = price.ToString(),
                                                    Otaqlar = node.SelectSingleNode("div[3]/ul/li[contains(text(),otaqlı)]").InnerText,
                                                    Kvadrat = kvadrat.InnerText,
                                                    ElanId = elanidnumber,
                                                };
                                                if (floor != null)
                                                {
                                                  
                                                        emlak.Floor = floor.InnerText.Trim();
                                                }

                                                if (date.Contains("bugün"))
                                                {
                                                    emlak.Date = date.Replace("bugün", DateTime.Now.ToString("dd-MM"));
                                                }
                                                else if (date.Contains("dünən"))
                                                {
                                                    var datetime = DateTime.Now.AddDays(-1);
                                                    emlak.Date = datetime.ToString("dd-MM");
                                                }
                                                else
                                                {
                                                    emlak.Date = date;

                                                }

                                                if (emlak.Type.ToLower().Contains('ə'))
                                                {
                                                    emlak.Type = emlak.Type.Replace("ə", "e");
                                                }
                                                if (emlak.Location.ToLower().Contains('ə'))
                                                {
                                                    emlak.Location = emlak.Location.Replace("ə", "e");
                                                    emlak.Location = emlak.Location.Replace("Ə", "e");

                                                }
                                                if (emlak.Date.ToLower().Contains('ə'))
                                                {
                                                    emlak.Date = emlak.Date.Replace("ə", "e");
                                                }
                                             
                                                if (!emlaklar.Select(x => x.ElanId).Contains(emlak.ElanId) && emlak.Location != "Sumqayıt")
                                                {
                                                    emlaklar.Add(emlak);
                                                    using (SqlCommand command = new SqlCommand($"Insert into Emlaklar (ElanId,ElanUrl,Price,Location,Otaqlar,Kvadrat,Date,Type,Floor) values('{emlak.ElanId}','{emlak.ElanUrl}',{emlak.Price},'{emlak.Location}','{emlak.Otaqlar}','{emlak.Kvadrat}','{emlak.Date}','{emlak.Type}','{emlak.Floor}')", consql))
                                                    {
                                                        command.ExecuteNonQuery();
                                                    }
                                                }
                                            

                                                Console.WriteLine("                    Price: " + price);
                                                var otaqlar = node.SelectSingleNode("div[3]/ul/li[contains(text(),otaqlı)]");
                                                var square = emlak.Kvadrat;

                                                if (otaqlar != null && otaqlar.InnerText.Contains("otaq"))
                                                {
                                                    Console.WriteLine("                    Rooms : " + node.SelectSingleNode("div[3]/ul/li[contains(text(),otaqlı)]").InnerText);
                                                    if (square != null)
                                                    {
                                                        Console.WriteLine("                    Square: " + square);
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("                    Sot: " + square);
                                                    }
                                                }
                                                else
                                                {
                                                    if (square != null)
                                                    {
                                                        Console.WriteLine("                    Square: " + square);
                                                    }
                                                }
                                                Console.WriteLine("                    City & Date : " + node.SelectSingleNode("div[3]/div[3]").InnerText + "\n");
                                                Console.WriteLine("---------------------------------------------------------------------------------------------------------------------");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                else
                {
                    Console.WriteLine("Elements not found on page " + pageNumber);
                    break;
                }


                pageNumber++;

            }


        }

        public static int getTotalX(string n, int k)
        {
            string concateds = string.Concat(Enumerable.Repeat(n, k).ToArray());
            int[] arr = new int[n.Length * k];
            arr = concateds.Select(x => int.Parse(x.ToString())).ToArray();
            do
            {
                if (arr.Sum() > 9)
                {
                    arr = arr.Sum().ToString().Select(x => int.Parse(x.ToString())).ToArray();
                }
                else
                {
                    return arr.Sum();
                }
            }
            while (true);
        }
        public static int superDigit(string n, int k)
        {
            int sum = 0;
            var cr = n.ToCharArray();
            var arr = new int[k];
            do
            {
                for (int i = 0; i < k; i++)
                {
                    arr[i] = int.Parse(cr[i].ToString());
                    sum += arr[i];
                }
                if (sum.ToString().Length > 1)
                {
                    cr = sum.ToString().ToCharArray();
                    //1-2
                    arr = new int[cr.Count() * k];
                    sum = 0;
                }
            } while (sum.ToString().Length < 2);
            return sum;
        }
        static bool GetIsVasiteci(string itemLink)
        {
            var web = new HtmlWeb();
            var doc = web.Load(itemLink);

            // Find the <div> element that indicates if the owner is "Vasiteci"
            var divElement = doc.DocumentNode.SelectSingleNode("//div[contains(text(), 'vasitəçi (agent)')]");

            // Check if the <div> element is found and if its inner text contains "vasiteci"
            return divElement != null && divElement.InnerText.Contains("vasitəçi (agent)");
        }
    }


    class ItemInfo
    {
        public bool IsVasiteci { get; set; }
    }


}



