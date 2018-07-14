using System;
using Microsoft.SPOT;
using System.Collections;
using Json.NETMF;

namespace System.Diagnostics
{
    public enum DebuggerBrowsableState
    {
        Never = 0,
        Collapsed = 2,
        RootHidden = 3
    }
}

namespace Test
{
    public class Program
    {
        public class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Address { get; set; }
            public DateTime Birthday { get; set; }
            public int ID { get; set; }
            public string[] ArrayProperty { get; set; }
            public Person Friend { get; set; }
        }

        public abstract class AbstractClass
        {
            public int ID { get; set; }
            public abstract string Test { get; }
            public virtual string Test2 { get { return "test2"; } }
        }

        public class RealClass : AbstractClass
        {
            public override string Test { get { return "test"; } }
        }

        public static void Main()
        {
            BasicSerializationTest();
            SerializeSimpleClassTest();
            BasicDeserializationTest();
            SerializeAbstractClassTest();
            SerializeStringsWithEscapeChars();
            SerializeDeserializeDateTest();
            SerializeNestedHashtablesTest();
            NestedHashtablesDeserializationTest();
        }

        public static bool SerializeStringsWithEscapeChars()
        {
            try
            {
                Hashtable hashTable = new Hashtable();
                hashTable.Add("quote", "---\"---");
                hashTable.Add("backslash", "---\\---");
                string json = JsonSerializer.SerializeObject(hashTable);
                string correctValue = "{\"quote\":\"---\\\"---\",\"backslash\":\"---\\\\---\"}";
                if (json != correctValue)
                {
                    Debug.Print("Fail: SerializeStringsWithEscapeChars - Values did not match");
                    return false;
                }

                Debug.Print("Success: SerializeStringsWithEscapeChars");
                return true;
            }
            catch (Exception ex)
            {
                Debug.Print("Fail: SerializeStringsWithEscapeChars - " + ex.Message);
                return false;
            }
        } 

        public static bool SerializeAbstractClassTest()
        {
            try
            {
                AbstractClass a = new RealClass() { ID = 12 };
                string json = JsonSerializer.SerializeObject(a);
                string correctValue = "{\"Test2\":\"test2\",\"ID\":12,\"Test\":\"test\"}";
                if (json != correctValue)
                {
                    Debug.Print("Fail: SerializeAbstractClassTest - Values did not match");
                    return false;
                }

                RealClass b = new RealClass() { ID = 12 };
                json = JsonSerializer.SerializeObject(b);
                correctValue = "{\"Test2\":\"test2\",\"ID\":12,\"Test\":\"test\"}";
                if (json != correctValue)
                {
                    Debug.Print("Fail: SerializeAbstractClassTest - Values did not match");
                    return false;
                }

                Debug.Print("Success: SerializeAbstractClassTest");
                return true;
            }
            catch (Exception ex)
            {
                Debug.Print("Fail: SerializeAbstractClassTest - " + ex.Message);
                return false;
            }
        }

        public static bool SerializeSimpleClassTest()
        {
            try
            {
                Person friend = new Person()
                {
                    FirstName = "Bob",
                    LastName = "Smith",
                    Birthday = new DateTime(1983, 7, 3),
                    ID = 2,
                    Address = "123 Some St",
                    ArrayProperty = new string[] { "hi", "planet" },
                };
                Person person = new Person() 
                { 
                    FirstName = "John", 
                    LastName = "Doe", 
                    Birthday = new DateTime(1988, 4, 23), 
                    ID = 27, 
                    Address = null,
                    ArrayProperty = new string[] { "hello", "world" },
                    Friend = friend
                };
                string json = JsonSerializer.SerializeObject(person);
                string correctValue = "{\"Address\":null,\"ArrayProperty\":[\"hello\",\"world\"],\"ID\":27,\"Birthday\":\"1988-04-23T00:00:00.000Z\",\"LastName\":\"Doe\",\"Friend\""
                    + ":{\"Address\":\"123 Some St\",\"ArrayProperty\":[\"hi\",\"planet\"],\"ID\":2,\"Birthday\":\"1983-07-03T00:00:00.000Z\",\"LastName\":\"Smith\",\"Friend\":null,\"FirstName\":\"Bob\"}"
                    +",\"FirstName\":\"John\"}";
                if (json != correctValue)
                {
                    Debug.Print("Fail: SerializeSimpleClassTest - Values did not match");
                    return false;
                }

                Debug.Print("Success: SerializeSimpleClassTest");
                return true;
            }
            catch (Exception ex)
            {
                Debug.Print("Fail: SerializeSimpleClassTest - " + ex.Message);
                return false;
            }
        } 

        public static bool BasicSerializationTest()
        {
            try
            {
                ICollection collection = new ArrayList() { 1, null, 2, "blah", false };
                Hashtable hashtable = new Hashtable();
                hashtable.Add("collection", collection);
                hashtable.Add("nulltest", null);
                hashtable.Add("stringtest", "hello world");
                object[] array = new object[] { hashtable };
                string json = JsonSerializer.SerializeObject(array);
                string correctValue = "[{\"stringtest\":\"hello world\",\"nulltest\":null,\"collection\":[1,null,2,\"blah\",false]}]";
                if (json != correctValue)
                {
                    Debug.Print("Fail: BasicSerializationTest - Values did not match");
                    return false;
                }

                Debug.Print("Success: BasicSerializationTest");
                return true;
            }
            catch (Exception ex)
            {
                Debug.Print("Fail: BasicSerializationTest - " + ex.Message);
                return false;
            }
        }

        public static bool BasicDeserializationTest()
        {
            try
            {
                string json = "[{\"stringtest\":\"hello world\",\"nulltest\":null,\"collection\":[-1,null,24.565657576,\"blah\",false]}]";
                ArrayList arrayList = JsonSerializer.DeserializeString(json) as ArrayList;
                Hashtable hashtable = arrayList[0] as Hashtable;
                string stringtest = hashtable["stringtest"].ToString();
                object nulltest = hashtable["nulltest"];
                ArrayList collection = hashtable["collection"] as ArrayList;
                long a = (long)collection[0];
                object b = collection[1];
                double c = (double)collection[2];
                string d = collection[3].ToString();
                bool e = (bool)collection[4];

                if (arrayList.Count != 1)
                {
                    Debug.Print("Fail: BasicDeserializationTest - Values did not match");
                    return false;
                }

                if (hashtable.Count != 3)
                {
                    Debug.Print("Fail: BasicDeserializationTest - Values did not match");
                    return false;
                }

                if (stringtest != "hello world")
                {
                    Debug.Print("Fail: BasicDeserializationTest - Values did not match");
                    return false;
                }

                if (nulltest != null)
                {
                    Debug.Print("Fail: BasicDeserializationTest - Values did not match");
                    return false;
                }

                if (collection.Count != 5)
                {
                    Debug.Print("Fail: BasicDeserializationTest - Values did not match");
                    return false;
                }

                if (a != -1)
                {
                    Debug.Print("Fail: BasicDeserializationTest - Values did not match");
                    return false;
                }

                if (b != null)
                {
                    Debug.Print("Fail: BasicDeserializationTest - Values did not match");
                    return false;
                }

                if (c != 24.565657576)
                {
                    Debug.Print("Fail: BasicDeserializationTest - Values did not match");
                    return false;
                }

                if (d != "blah")
                {
                    Debug.Print("Fail: BasicDeserializationTest - Values did not match");
                    return false;
                }

                if (e != false)
                {
                    Debug.Print("Fail: BasicDeserializationTest - Values did not match");
                    return false;
                }

                Debug.Print("Success: BasicDeserializationTest");
                return true;
            }
            catch (Exception ex)
            {
                Debug.Print("Fail: BasicDeserializationTest - " + ex.Message);
                return false;
            }
        }

        public static bool SerializeDeserializeDateTest()
        {
            try
            {
                DateTime testTime = new DateTime(2015, 04, 22, 11, 56, 39, 456);
                JsonSerializer dataSerializer = new JsonSerializer(DateTimeFormat.ISO8601);
                string jsonString = dataSerializer.Serialize(testTime);
                string deserializedJsonString = (string)dataSerializer.Deserialize(jsonString);
                DateTime convertTime = DateTimeExtensions.FromIso8601(deserializedJsonString);

                if (testTime != convertTime)
                {
                    Debug.Print("Fail: SerializeDeserializeDateTest - Values did not match");
                    return false;
                }

                Debug.Print("Success: SerializeDeserializeDateTest");
                return true;
            }
            catch (Exception ex)
            {
                Debug.Print("Fail: SerializeDeserializeDateTest - " + ex.Message);
                return false;
            }
        }

        public static bool SerializeNestedHashtablesTest()
        {
            try
            {
                Hashtable leaf11 = new Hashtable();
                leaf11.Add("name", "labels");
                leaf11.Add("value", "[]");
                Hashtable leaf12 = new Hashtable();
                leaf12.Add("name", "series");
                leaf12.Add("value", "0");
                ArrayList leafs1 = new ArrayList() { leaf11, leaf12 };
                Hashtable branch1 = new Hashtable();
                branch1.Add("card_type", "chart-donut");
                branch1.Add("total", 100);
                branch1.Add("units", "%");
                branch1.Add("values", leafs1);
                branch1.Add("is_series", true);
                branch1.Add("max",12);
                branch1.Add("low", 0);
                branch1.Add("high", 100);
                Hashtable leaf21 = new Hashtable();
                leaf21.Add("name", "labels");
                leaf21.Add("value", "[]");
                Hashtable leaf22 = new Hashtable();
                leaf22.Add("name", "series");
                leaf22.Add("value", "0");
                ArrayList leafs2 = new ArrayList() { leaf21, leaf22 };
                Hashtable branch2 = new Hashtable();
                branch2.Add("card_type", "chart-donut");
                branch2.Add("total", 80);
                branch2.Add("units", "Celcius");
                branch2.Add("values", leafs2);
                branch2.Add("is_series", true);
                branch2.Add("max", 12);
                branch2.Add("low", 0);
                branch2.Add("high", 100);
                Hashtable root = new Hashtable();
                root.Add("0", branch1);
                root.Add("1", branch2);
                string json = JsonSerializer.SerializeObject(root);
                string correctValue = "{\"0\":{\"units\":\"%\",\"card_type\":\"chart-donut\",\"total\":100,\"max\":12,\"is_series\":true,\"low\":0,\"high\":100,\"values\":[{\"name\":\"labels\",\"value\":\"[]\"},{\"name\":\"series\",\"value\":\"0\"}]},\"1\":{\"units\":\"Celcius\",\"card_type\":\"chart-donut\",\"total\":80,\"max\":12,\"is_series\":true,\"low\":0,\"high\":100,\"values\":[{\"name\":\"labels\",\"value\":\"[]\"},{\"name\":\"series\",\"value\":\"0\"}]}}";
                if (json != correctValue)
                {
                    Debug.Print("Fail: SerializeNestedHashtablesTest - Values did not match");
                    return false;
                }
                Debug.Print("Success: SerializeNestedHashtablesTest");
                return true;
            }
            catch (Exception ex)
            {
                Debug.Print("Fail: SerializeNestedHashtablesTest - " + ex.Message);
                return false;
            }
        }

        public static bool NestedHashtablesDeserializationTest()
        {
            try
            {
                string json = "{\"0\":{\"units\":\"%\",\"card_type\":\"chart-donut\",\"total\":100,\"max\":12,\"is_series\":true,\"low\":0,\"high\":100,\"values\":[{\"name\":\"labels\",\"value\":\"[]\"},{\"name\":\"series\",\"value\":\"0\"}]},\"1\":{\"units\":\"Celcius\",\"card_type\":\"chart-donut\",\"total\":80,\"max\":12,\"is_series\":true,\"low\":0,\"high\":100,\"values\":[{\"name\":\"labels\",\"value\":\"[]\"},{\"name\":\"series\",\"value\":\"0\"}]}}";
                Hashtable root = JsonSerializer.DeserializeString(json) as Hashtable;
                Hashtable branch1 = root["0"] as Hashtable;
                Hashtable branch2 = root["1"] as Hashtable;
                ArrayList leafs1 = branch1["values"] as ArrayList;
                ArrayList leafs2 = branch2["values"] as ArrayList;
                int branch1Total =Convert.ToInt16(branch1["total"].ToString());
                int branch2Total = Convert.ToInt16(branch2["total"].ToString());

                if (root.Count != 2)
                {
                    Debug.Print("Fail: NestedHashtablesDeserializationTest - Values did not match");
                    return false;
                }

                if (branch1.Count != 8)
                {
                    Debug.Print("Fail: NestedHashtablesDeserializationTest - Values did not match");
                    return false;
                }

                if (branch2.Count != 8)
                {
                    Debug.Print("Fail: NestedHashtablesDeserializationTest - Values did not match");
                    return false;
                }

                if (leafs1.Count != 2)
                {
                    Debug.Print("Fail: NestedHashtablesDeserializationTest - Values did not match");
                    return false;
                }

                if (leafs2.Count != 2)
                {
                    Debug.Print("Fail: NestedHashtablesDeserializationTest - Values did not match");
                    return false;
                }

                if (branch1Total != 100)
                {
                    Debug.Print("Fail: NestedHashtablesDeserializationTest - Values did not match");
                    return false;
                }

                if (branch2Total != 80)
                {
                    Debug.Print("Fail: NestedHashtablesDeserializationTest - Values did not match");
                    return false;
                }

                Debug.Print("Success: NestedHashtablesDeserializationTest");
                return true;
            }
            catch (Exception ex)
            {
                Debug.Print("Fail: NestedHashtablesDeserializationTest - " + ex.Message);
                return false;
            }
        }

    }
}
