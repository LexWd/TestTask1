using System;
using System.Collections ;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TestTask1.Properties ;

namespace TestTask1
{
	public class TagStorage
	{
		private const string StartFile = "./Storage.xml";
		private const string NewFile = "./NewStorage.xml";
		public TagItem TagRoot;
		private XmlDocument _storage;
	    private readonly List<XmlNode> _tempList2 = new List<XmlNode> ();

        public TagStorage()
		{
			Load();
            var temp = Write();
		    if ( temp )
		    {
                Console.WriteLine("Successful load");
		        Save(TagRoot, null);
            }
		    else
		    {
		        Console.WriteLine("Error in loading");
            }
		}

		private void Load()
		{
			_storage = new XmlDocument();
			_storage.Load(StartFile);
		}

		public void Save(TagItem root, [CanBeNull] XmlElement parent)
		{
			_storage = new XmlDocument();
		    var temp = _storage.CreateXmlDeclaration ( "1.0", "UTF-8", "" ) ;
		   var tempRoot = _storage.CreateElement ( "tags" ) ;
		    _storage.AppendChild ( temp ) ;
		    _storage.AppendChild ( tempRoot ) ;
			CreateStructure(root, tempRoot);
			_storage.Save(NewFile);
		}

		private void CreateStructure(TagItem root, [CanBeNull] XmlElement parent)
		{
			var tag = _storage.CreateElement("tag");
			var nameAttr = _storage.CreateAttribute("name");
			var pathAttr = _storage.CreateElement("FullPath");
			var levelAttr = _storage.CreateElement("Level");
			var valueTypeAttr = _storage.CreateElement("ValueType");
			var valueAttr = _storage.CreateElement("Value");

			var nameText = _storage.CreateTextNode(root.GetName());
			var pathText = _storage.CreateTextNode(root.GetFullPath());
			var levelText = _storage.CreateTextNode(root.GetLevel().ToString());
			var valueTypeText = _storage.CreateTextNode(TagItem.GetTypeOfValue(root));
			var valueText = _storage.CreateTextNode(root.GetValue().ToString());

			nameAttr.AppendChild(nameText);
			pathAttr.AppendChild(pathText);
			levelAttr.AppendChild(levelText);
			valueAttr.AppendChild(valueText);
			valueTypeAttr.AppendChild(valueTypeText);
			tag.Attributes.Append(nameAttr);
			tag.AppendChild(pathAttr);
			tag.AppendChild(levelAttr);
			tag.AppendChild(valueAttr);
			tag.AppendChild(valueTypeAttr);

		    if (root.HaveChilds())
			{
				var childElement = _storage.CreateElement("Childs");
				foreach (var child in root.GetChilds()) CreateStructure(child, childElement);
				tag.AppendChild(childElement);
			}

			if (parent != null)
				parent.AppendChild(tag);
			else
				_storage.AppendChild(tag);
		}

		private bool Write()
		{
			var xRoot = _storage.DocumentElement;
			if (xRoot == null) return false;
			TagRoot = CreateData( null, null);
		    return TagRoot != null ;
		}

		private static void WriteNodes(IEnumerable xRoot)
		{

			foreach (XmlNode xNode in xRoot)
			{
				if (xNode.Attributes == null || xNode.Attributes.Count <= 0) continue;
				var attr = xNode.Attributes.GetNamedItem("name");
				if (attr != null)
					Console.WriteLine(attr.Value);
				foreach (XmlNode childNode in xNode.ChildNodes)
					switch (childNode.Name)
					{
						case "Level":
							Console.WriteLine($"Level: {childNode.InnerText}");
							break;
						case "FullPath":
							Console.WriteLine($"Full Path: {childNode.InnerText}");
							break;
						case "Value":
							Console.WriteLine($"Value: {childNode.InnerText}");
							break;
						case "ValueType":
							Console.WriteLine($"ValueType: {childNode.InnerText}");
							break;
						case "Childs":
							var node = (XmlElement)childNode;
							Console.WriteLine();
							WriteNodes(node);
							break;
						default:
							Console.WriteLine("Incorrect parameter");
							break;
					}

				Console.WriteLine();
			}
		}


	    private TagItem CreateData ([CanBeNull] TagItem parentItem,[CanBeNull] XmlNode nodeForItem)
	    {
	        if ( nodeForItem == null )
	        {
	            nodeForItem = _storage.DocumentElement ;
	            nodeForItem = nodeForItem?.FirstChild ;
	        }else if ( _tempList2.Contains ( nodeForItem ) ) return null ;

	        if ( nodeForItem?.Attributes == null || (nodeForItem.Name != "tag"))
	        {
	            return null ;
	        }
	        else
	        {
	            _tempList2.Add(nodeForItem);
	            var  attr      = nodeForItem.Attributes.GetNamedItem("name");
	            uint tempLevel = 1;
	            var  tempPath  = "";
	            var  tempName  = attr.Value;
	            var  tempValue = "";
	            var  valueType = "";
	            XmlNode childsNod = null ;
                foreach (XmlNode childNode in nodeForItem.ChildNodes)
                { 
	                switch (childNode.Name)
	                {

	                    case "Level":
	                        tempLevel = uint.Parse(childNode.InnerText);
	                        break;
	                    case "FullPath":
	                        tempPath = childNode.InnerText;
	                        break;
	                    case "Value":
	                        tempValue = childNode.InnerText;
	                        break;
	                    case "ValueType":
	                        valueType = childNode.InnerText;
	                        break;
	                    case "Childs":
	                        childsNod = childNode ;
	                        break;
	                    default:
	                        Console.WriteLine("Incorrect parameter");
	                        break;
	                }
	            }
                TagItem newItem = new TagItem(tempName,parentItem,tempValue,valueType,tempPath,tempLevel);

	            if ( childsNod != null )
	            {
	                foreach (XmlNode child in childsNod.ChildNodes )
	                {
	                    var tempItem = CreateData ( newItem, child ) ;
	                    if ( tempItem != null )
	                    {
                            newItem.AddChild(tempItem);
	                    }
	                }
	            }

	            return newItem ;
	        }
	    }

		public TagItem Search(string fullName)
		{
			var tempItem = TagRoot;
			var split = fullName.Trim().Split('.');

			return split.Aggregate(tempItem, (current, name) => current.FindChild(name));
		}

		public static void DeleteTag(TagItem tagForDelete)
		{
			tagForDelete?.GetParent()?.DeleteChild(tagForDelete);
		}
	}
}