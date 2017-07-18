using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
/// <summary>
/// Summary description for Apriori
/// </summary>
public class Apriori
{
    int FLAG = 0;
    List<string> candidates = new List<string>();
    public static Dictionary<string, int> dicitemsupport = new Dictionary<string, int>();
    List<int> countfrequency = new List<int>();
    internal List<string> lstitems = new List<string>();
    internal List<string> dlm1 = new List<string>();
    public List<string> dlm2 = new List<string>();
    public static List<string> finalCandidate = new List<string>();
    public List<string> finalitemset = new List<string>();
    public double totalconfidance = 0;
    internal int numItems; //number of items per transaction
    internal int numTransactions; //number of transactions
    internal double minSup; //minimum support for a frequent itemset
    internal List<string> oneVal; //array of value per column that will be treated as a '1'
    internal string itemSep = " "; //the separator value for items in the database
    internal Random rm = new Random();
    internal List<List<string>> MyList;
    public static double Acc = 0;
    public static long Etime = 0;
    public static double Mem = 0;
    public static double errr = 0;
    internal int sum = 0;
    internal int Tctr = 0;
    internal int Fctr = 0;

    internal int tempsize = 0;

    public Apriori(int numItem, int numTrans, int minsup, List<string> itemset, List<List<string>> transactionset, List<string> candidate)
    {

        numItems = numItem;
        numTransactions = numTrans;
        minSup = minsup;
        MyList = transactionset;
        oneVal = itemset;
        //minConfidence=confidence;
        candidates = candidate;
        //FLAG = aflg;
    }


    public virtual List<string> aprioriProcess()
    {
        int itemsetNumber = 0; //the current itemset being looked at

        //Show the default Config*******************
        dlm1.Add("\nDefault Configuration: ");

        dlm1.Add("\nInput configuration: " + numItems + " items " + MyList.Count + " transactions");
        dlm1.Add("\nminsup = " + minSup);
        dlm1.Add("\n");

        //***************************
        dlm1.Add("\n APROIRI algorithm has started.\n");
        dlm1.Add("\nItems:");
        for (int i = 0; i < oneVal.Count; i++)
        {
            dlm1.Add(oneVal[i] + "\n");
        }
        //while not complete
        do
        {
            //increase the itemset that is being looked at
            itemsetNumber++;

            //generate the candidates
            generateCandidates(itemsetNumber);

            //determine and display frequent itemsets

            if (candidates.Count != 0)
            {
                dlm1.Add("\nFrequent " + itemsetNumber + "-itemsets");
                calculateFrequentItemsets(itemsetNumber);
            }

        } while (candidates.Count > 1);
     
        return dlm1;
    }

    private void generateCandidates(int n)
    {
        List<string> tempCandidates = new List<string>();
        List<string> lstitem = new List<string>();
        countfrequency = new List<int>();
        string str1, str2; //strings that will be used for comparisons
        string it1, it2;
        StringTokenizer st1, st2; //string tokenizers for the two itemsets being compared
        StringTokenizer item1, item2;
        //if its the first set, candidates are just the numbers
        if (n == 1)
        {
            for (int i = 0; i < numItems; i++)
            {
                int count = 0;
                string item = "";
                for (int k = 0; k < MyList.Count; k++)
                {
                    //string s = MyList[k][Convert.ToInt32(candidates[i])];
                    if (MyList[k].Contains(oneVal[i]))
                    {
                        //count = count + Convert.ToInt32(MyList[k][Convert.ToInt32(candidates[i])]);
                        count++;
                        item = oneVal[i];
                        Tctr++;
                    }
                    else
                    {
                        Fctr++;
                    }

                }

                if (count >= minSup)
                {
                    tempCandidates.Add(candidates[i]);
                    countfrequency.Add(count);
                    lstitem.Add(item);
                    if (!dicitemsupport.ContainsKey(item))
                    {
                        dicitemsupport.Add(item, count);
                    }
                }
            }

        }

        else if (n == 2) //second itemset is just all combinations of itemset 1
        {
            //add each itemset from the previous frequent itemsets together
            for (int i = 0; i < candidates.Count; i++)
            {
                st1 = new StringTokenizer(candidates[i]);
                string itm1 = lstitems[i];
                str1 = st1.NextToken();
                for (int j = i + 1; j < candidates.Count; j++)
                {
                    st2 = new StringTokenizer(candidates[j]);
                    str2 = st2.NextToken();
                    string itm2 = lstitems[j];
                    int countoccurance = 0;
                    for (int x = 0; x < MyList.Count; x++)
                    {
                        //if (MyList[x][Convert.ToInt32(str1.Trim())].Equals(itm1) && MyList[x][Convert.ToInt32(str2.Trim())].Equals(itm1))
                        //{
                        if (MyList[x].Contains(itm1) && MyList[x].Contains(itm2))
                        {
                            countoccurance++;
                            /* int a = Convert.ToInt32(MyList[x][Convert.ToInt32(str1.Trim())]);
                             int b = Convert.ToInt32(MyList[x][Convert.ToInt32(str2.Trim())]);
                             if (a > b)
                             {
                                 countoccurance = countoccurance + (a - b);
                             }
                             else if (a < b)
                             {
                                 countoccurance = countoccurance + (b - a);
                             }
                             else
                             {
                                 countoccurance = countoccurance + a;
                             }*/
                        }
                    }
                    if (countoccurance >= minSup)
                    {
                        tempCandidates.Add(str1 + " " + str2);
                        lstitem.Add(itm1 + " " + itm2);
                        countfrequency.Add(countoccurance);
                        if (!dicitemsupport.ContainsKey(itm1 + " " + itm2))
                        {
                            dicitemsupport.Add(itm1 + " " + itm2, countoccurance);
                        }
                    }

                }

            }
        }

        else
        {
            //for each itemset
            //for each itemset
            for (int i = 0; i < candidates.Count; i++)
            {
                //compare to the next itemset
                for (int j = i + 1; j < candidates.Count; j++)
                {
                    //create the strings
                    str1 = "";
                    str2 = "";
                    it1 = "";
                    it2 = "";

                    //create the tokenizers
                    st1 = new StringTokenizer(candidates[i]);
                    st2 = new StringTokenizer(candidates[j]);
                    item1 = new StringTokenizer(lstitems[i]);
                    item2 = new StringTokenizer(lstitems[j]);

                    //make a string of the first n-2 tokens of the strings
                    for (int s = 0; s < n - 2; s++)
                    {
                        str1 = str1 + " " + st1.NextToken();
                        str2 = str2 + " " + st2.NextToken();
                        it1 = it1 + " " + item1.NextToken();
                        it2 = it2 + " " + item2.NextToken();
                    }

                    //if they have the same n-2 tokens, add them together
                    if (str2.CompareTo(str1) == 0)
                    {
                        string candidatestr = (str1 + " " + st1.NextToken() + " " + st2.NextToken()).Trim();
                        string itemstr = (it1 + " " + item1.NextToken() + " " + item2.NextToken()).Trim();
                        string[] resultitemstr = itemstr.Split(new string[] { " " }, StringSplitOptions.None);
                        string[] resultindexstr = candidatestr.Split(new string[] { " " }, StringSplitOptions.None);
                        int countoccurance = 0;
                        for (int s = 0; s < MyList.Count; s++)
                        {
                            int tempcount = 0;
                            int[] itemcnt = new int[resultindexstr.Length];
                            for (int x = 0; x < resultindexstr.Length; x++)
                            {
                                if (MyList[s].Contains(resultitemstr[x].Trim()))
                                {
                                    //tempcount=tempcount+Integer.parseInt(MyList.get(s).get(Integer.parseInt(resultindexstr[x].trim())));
                                    tempcount++;
                                    //itemcnt[x] = Convert.ToInt32(MyList[s][Convert.ToInt32(resultindexstr[x].Trim())]);
                                }
                            }
                            if (tempcount == resultitemstr.Length)
                            {
                                //countoccurance = countoccurance + itemcnt[getMinIndex(itemcnt)];
                                countoccurance++;
                            }
                        }
                        if (countoccurance >= minSup)
                        {
                            tempCandidates.Add(candidatestr);
                            lstitem.Add(itemstr);
                            countfrequency.Add(countoccurance);
                            if (!dicitemsupport.ContainsKey(itemstr))
                            {
                                dicitemsupport.Add(itemstr, countoccurance);
                            }
                        }
                    }

                }
            }
        }
        //clear the old candidates
        candidates.Clear();
        lstitems.Clear();
        candidates = new List<string>(tempCandidates);
        lstitems = new List<string>(lstitem);
        tempCandidates.Clear();
        lstitem.Clear();

    }
    public int getMinIndex(int[] ar)
    {
        int min = ar[0];
        int index = 0;
        for (int i = 0; i < ar.Length; i++)
        {
            if (min > ar[i])
            {
                min = ar[i];
                index = i;
            }
        }
        return index;
    }

    private void calculateFrequentItemsets(int n)
    {

        finalCandidate = new List<string>(candidates);
        finalitemset = new List<string>(lstitems);
        if (countfrequency.Count >= 1)
        {
            for (int g = 0; g < countfrequency.Count; g++)
            {
                if (countfrequency[g] != 0)
                {
                    string Myl = "\n" + "Frequent Item Set: { " + lstitems[g] + "} Found " + countfrequency[g] + " Times";
                    dlm1.Add(Myl);
                }

            }
        }
    }

    public static List<List<string>> getTransaction(List<string> data)
    {
        List<List<string>> lsttransaction = new List<List<string>>();
        for (int i = 0; i < data.Count; i++)
        {
            string[] row = data[i].Trim().Split(new string[] { " " }, StringSplitOptions.None);
            List<string> instance = new List<string>();
            for (int j = 0; j < row.Length; j++)
            {
                instance.Add(row[j].Trim());
            }
            lsttransaction.Add(instance);
        }
        return lsttransaction;
    }
    public static List<string> getItems(List<string> data)
    {
        List<string> lstitems = new List<string>();
        for (int i = 0; i < data.Count; i++)
        {
            string[] row = data[i].Trim().Split(new string[] { " " }, StringSplitOptions.None);
            List<string> instance = new List<string>();
            for (int j = 0; j < row.Length; j++)
            {
                if (!lstitems.Contains(row[j].Trim()) && !row[j].Trim().Equals(""))
                    lstitems.Add(row[j].Trim());
            }

        }
        return lstitems;
    }

    public List<string> generateRules(Dictionary<string, int> dicitemwithsupport, int minConfidence)
    {
        List<string> Rules = new List<string>();
        int confidencecount = 0;
        double confidencesum = 0.0;
        dlm2.Add("ASSOCIATION RULES:-");
        dlm2.Add("\n");
        List<string> lststr = dicitemwithsupport.Keys.ToList();
        for (int i = 0; i < lststr.Count; i++)
        {
            for (int j = i + 1; j < lststr.Count; j++)
            {
                for (int k = j + 1; k < lststr.Count; k++)
                {
                    int flag = 0;
                    string[] item1 = lststr[i].Split(' ');
                    string[] item2 = lststr[j].Split(' ');
                    for (int m = 0; m < item1.Length; m++)
                    {
                        if (lststr[j].Contains(item1[m]))
                        {
                            flag = 1;
                            break;
                        }

                    }
                    if (flag != 1)
                    {
                        string[] item3 = lststr[k].Split(' ');
                        if (item1.Length + item2.Length == item3.Length)
                        {
                            if (dicitemwithsupport.ContainsKey(lststr[i] + " " + lststr[j]))
                            {
                                for (int x = 0; x < item1.Length; x++)
                                {
                                    if (lststr[k].Contains(item1[x]))
                                    {
                                        flag = 0;
                                    }
                                    else
                                    {
                                        flag = 1;
                                        break;
                                    }
                                }
                                if (flag != 1)
                                {
                                    for (int x = 0; x < item2.Length; x++)
                                    {
                                        if (lststr[k].Contains(item2[x]))
                                        {
                                            flag = 0;
                                        }
                                        else
                                        {
                                            flag = 1;
                                            break;
                                        }
                                    }
                                }
                                if(flag != 1)
                                {
                                    double support = dicitemwithsupport[lststr[i] + " " + lststr[j]];
                                    int support1 = dicitemwithsupport[lststr[i]];
                                    int support2 = dicitemwithsupport[lststr[j]];
                                    double finalsupport1 = (support / support1) * 100;
                                    double finalsupport2 = (support / support2) * 100;
                                    if (finalsupport1 >= minConfidence)
                                    {
                                        if (!Rules.Contains(lststr[i] + "----->" + lststr[j]))
                                        Rules.Add(lststr[i] + "----->" + lststr[j]);
                                    }
                                    if (finalsupport2 >= minConfidence)
                                    {
                                        if (!Rules.Contains(lststr[j] + "----->" + lststr[i]))
                                        Rules.Add(lststr[j] + "----->" + lststr[i]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        totalconfidance = confidencesum / confidencecount;
        return Rules;
    }

}