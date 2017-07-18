using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Net;
using System.Text;
/// <summary>
/// Summary description for MyWebService
/// </summary>
[WebService(Namespace = "http://mytest.org/",
Description = "My Apriori Service",
Name = "MyService")]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class MyWebService : System.Web.Services.WebService {

    //http://tempuri.org/;
    List<string> transactions = new List<string>();
    public Dictionary<string, string> dicAttribute = new Dictionary<string, string>();
    public SortedDictionary<string, string> sorteddic;
    public static List<string> AttributeWithAes = new List<string>();
    public static List<List<string>> lstdata = new List<List<string>>();
    Dictionary<string, string> aeswithencodedattribute = new Dictionary<string, string>();
    List<List<string>> lsttransaction;
    List<string> lstitems;
    List<string> lstcandidate = new List<string>();
    StringBuilder sb;
    string strmerge = "";
    public static  string rules = "";
    public static int index = 0;
    public static int minsupport;
    public static int minconfidence;
    /// <summary>
    public MyWebService () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

   [WebMethod]
    public int startServer(int support,int confidence)
    {
        minsupport = support;
        minconfidence = confidence;
        int num = 0;
        try
        {
            Random r = new Random();
            num = r.Next(1000, 9999);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ops! Error! " + ex.StackTrace);
        }
        return num;
    }

    [WebMethod]
   public void getDataFromClient(string data,string datawithaes)
   {
       try
       {
           string[] attributeAes = datawithaes.Split(new string[] { "\n" }, StringSplitOptions.None);
           for (int i = 0; i < attributeAes.Length - 1; i++)
           {
               string attribute = attributeAes[i].Substring(0, attributeAes[i].IndexOf(" "));
               string value = attributeAes[i].Substring(attributeAes[i].IndexOf(" ") + 1);
               dicAttribute.Add(attribute, value);
           }
           string[] databyline =data.Split(new string[] { "\n" }, StringSplitOptions.None);
           for (int i = 0; i < databyline.Length; i++)
           {
               transactions.Add(databyline[i]);
           }
       }
       catch (Exception ex)
       {
           Console.WriteLine("Ops! Error! " + ex.StackTrace);
       }
       lstdata.Add(transactions);

       foreach(string value in dicAttribute.Values)
       {
           AttributeWithAes.Add(value);
       }

        if(index>0)
        {
            sb = new StringBuilder();
            for (int i = 0; i < lstdata[0].Count; i++)
            {
                if (i == lstdata[0].Count - 1)
                {
                    sb.Append((lstdata[0][i] + " " + lstdata[1][i]).ToString());
                    strmerge = (lstdata[0][i] + " " + lstdata[1][i]).ToString();
                }
                else
                    sb.Append((lstdata[0][i] + " " + lstdata[1][i] + "\n").ToString());
            }

            generateRules();

        }
       index++;
   }
   
    public void generateRules()
    {
        List<string> items = new List<string>();
        List<int> itemindex = new List<int>();
        List<string> lstmodifiedtransactions = new List<string>();
        Random r = new Random();
        string[] s = strmerge.Split(new string[] { " " }, StringSplitOptions.None);
        int i = 0;
        while (i < AttributeWithAes.Count)
        {
            int num = r.Next(1,30);
            if (!itemindex.Contains(num))
            {
                aeswithencodedattribute.Add("A" + num, AttributeWithAes[i]);
                items.Add("A" + num);
                itemindex.Add(num);
                i++;
            }
        }
        string[] datavector = sb.ToString().Split(new string[] { "\n" }, StringSplitOptions.None);
        for (int j = 0; j < datavector.Length; j++)
        {
            string[] vectordata = datavector[j].Split(new string[] { " " }, StringSplitOptions.None);
            string s1 = "";
            for (int k = 0; k < vectordata.Length; k++)
            {
                string k1 = vectordata[k];
                if (vectordata[k] == "1")
                {
                    if (k == 0)
                        s1 = s1 + items[0] + " ";
                    if (k == 1)
                        s1 = s1 + items[1] + " ";
                    if (k == 2)
                        s1 = s1 + items[2] + " ";
                    if (k == 3)
                        s1 = s1 + items[3] + " ";
                    if (k == 5)
                        s1 = s1 + items[4] + " ";
                    if (k == 6)
                        s1 = s1 + items[5] + " ";
                    if (k == 7)
                        s1 = s1 + items[6] + " ";

                }
                if (vectordata[k] == "0")
                {
                    s1 = s1 + "";
                }
            }
            lstmodifiedtransactions.Add(s1);
        }
        lsttransaction = Apriori.getTransaction(lstmodifiedtransactions);
        lstitems = Apriori.getItems(lstmodifiedtransactions);
        for (int l = 0; l < lstitems.Count; l++)
        {
            lstcandidate.Add(l.ToString());
            sb.AppendLine(lstitems[l]);
        }

        Apriori apriori = new Apriori(lstitems.Count, lsttransaction.Count,minsupport, lstitems, lsttransaction, lstcandidate);
        List<string> lstresult = apriori.aprioriProcess();
        for (int r1 = 0; r1 < lstresult.Count; r1++)
        {
            sb.AppendLine(lstresult[r1]);
        }
        List<string> Rules = apriori.generateRules(Apriori.dicitemsupport,minconfidence);
        for (int i1 = 0; i1 < Rules.Count; i1++)
        {
            foreach (string attribute in aeswithencodedattribute.Keys)
            {
                if (Rules[i1].Contains(attribute))
                {
                    Rules[i1] = Rules[i1].Replace(attribute, aeswithencodedattribute[attribute]);
                }
            }
        }
       
        for (int i3 = 0; i3 < Rules.Count; i3++)
        {
            if (i3 == Rules.Count - 1)
                rules = rules + Rules[i3];
            else
                rules = rules + Rules[i3] + "\n";
        }

    }
    [WebMethod]
    public string sendRules()
    {
        return rules;
    }
}
