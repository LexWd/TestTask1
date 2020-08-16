using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using TestTask1.Properties ;

namespace TestTask1
{
	public sealed class TagItem : INotifyPropertyChanged
	{
		private string _name;

		// Since an object can have one of four value types, 
		// it is easier to store the value itself in the string 
		private string _value;

		private TagType _valueType;

		// In the case of the root of the hierarchy, there is no maternal object
		[CanBeNull] private TagItem _parent;

		// The list of child objects will be empty until we add any object to it.
		// Based on this, we can initialize this list here.
		private readonly List<TagItem> _childs;

		private string _fullPath;
		private uint _level = 1;

		#region InterfaceImplementation

		// In an effort to avoid processing an entire array of children using a loop, we can use an interface that notifies of changes in the fields

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void ParentOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "Name":
					ChangeFullPath();
					break;
				case "FullPath":
					ChangeFullPath();
					break;
			}
		}

		#endregion

		#region Constuctors

		private void Base(string name, [CanBeNull] TagItem parent) //Base part of any constructor
		{
			_name = name;
			_parent = parent;
			if (_parent == null) return;
			_level = _parent._level + 1;
			_parent.PropertyChanged += ParentOnPropertyChanged;
		}

		// Constructor for objects with int type values
		public TagItem(string name, [CanBeNull] TagItem parent, int value)
		{
			_value = value.ToString();
			_valueType = TagType.Int;
			_childs = new List<TagItem>();
			Base(name, parent);
		}

		// Constructor for objects with double type values
		public TagItem(string name, [CanBeNull] TagItem parent, double value)
		{
			_value = value.ToString(CultureInfo.InvariantCulture);
			_valueType = TagType.Double;
			Base(name, parent);
		}

		// Constructor for objects with bool type values
		public TagItem(string name, [CanBeNull] TagItem parent, bool value)
		{
			_value = value.ToString();
			_valueType = TagType.Bool;
			Base(name, parent);
		}

		// Constructor for objects without value
		public TagItem(string name, [CanBeNull] TagItem parent)
		{
			_value = null;
			_valueType = TagType.None;
			Base(name, parent);
		}

		// Constructor for root objects
		public TagItem(string name)
		{
			_value = null;
			_valueType = TagType.None;
			Base(name, null);
		}

		public TagItem(string name, [CanBeNull] TagItem parent, string tempValue, string valueType,
			string tempFullPath, uint tempLevel)
		{
			_name = name;
			_value = tempValue;
			_fullPath = tempFullPath;
			_level = tempLevel;
            _childs = new List<TagItem> ();
			_parent = parent;

		    try
		    {
		        _valueType = TagItemExtensions.ConvertStringToTagType(valueType);
		    }
		    catch (NotSupportedTagTypeException)
		    {
		        _valueType = TagType.None;
		    }

			if (_parent != null)
				_parent.PropertyChanged += ParentOnPropertyChanged;
		}

		public TagItem(string name, TagItem parent, TagType type)
		{
			_name = name;
			_parent = parent;
			_parent?.AddChild(this);
			_valueType = type;
		    _childs = new List<TagItem> ();
            if (_parent != null) _fullPath = _parent._fullPath + $".{name}";
			_level = parent.GetLevel() + 1;
			if (_parent != null) _parent.PropertyChanged += ParentOnPropertyChanged;
		}

		#endregion

		#region ChildsControll

		public void AddChild(TagItem newChild)
		{
			_childs.Add(newChild);
		    newChild._parent = this ;
		}

		// This method removes a descendant from its copy
		public void DeleteChild(TagItem child)
		{
			_childs.Remove(child);
		}

		// This method removes a child by name
		public void DeleteChild(string name)
		{
			_childs.Remove(FindChild(name));
		}

		// This method can find child of object by name and return reference to it
		public TagItem FindChild([NotNull] string name)
		{
			return _childs.Find(item => item._name == name);
		}

		public bool HaveChilds()
		{
			return _childs?.Count > 0;
		}

		public IEnumerable<TagItem> GetChilds()
		{
			return _childs;
		}

		#endregion

		#region FieldControll

		private TagType GetTypeOfValue()
		{
			return _valueType;
		}

		public static string GetTypeOfValue(TagItem tag)
		{
			switch (tag.GetTypeOfValue())
			{
				case TagType.Bool:
					return TagType.Bool.ToString();
				case TagType.Int:
					return "Int";
				case TagType.Double:
					return "Double";
				case TagType.None:
					return "None";
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public object GetValue()
		{
			switch (_valueType)
			{
				case TagType.Bool:
					return GetBoolValue();
				case TagType.Int:
					return GetIntValue();
				case TagType.Double:
					return GetDoubleValue();
				case TagType.None:
					return GetNoneValue();
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private int GetIntValue()
		{
			return Convert.ToInt32(_value);
		}

		private bool GetBoolValue()
		{
			return Convert.ToBoolean(_value);
		}

		private double GetDoubleValue()
		{
			return Convert.ToDouble(_value);
		}

		private static string GetNoneValue()
		{
			return "";
		}

		public void SetValue(int newValue)
		{
			_value = newValue.ToString();
			_valueType = TagType.Int;
		}

		public void SetValue(bool newValue)
		{
			_value = newValue.ToString();
			_valueType = TagType.Bool;
		}

		public void SetValue(double newValue)
		{
			_value = newValue.ToString(CultureInfo.InvariantCulture);
			_valueType = TagType.Double;
		}

		public void SetValue()
		{
			_value = null;
			_valueType = TagType.None;
		}

		public uint GetLevel()
		{
			return _level;
		}

		public string GetFullPath()
		{
			return _fullPath;
		}

		public string GetName()
		{
			return _name;
		}

		public TagItem GetParent()
		{
			return _parent;
		}

		#region Renaming

		// Object name change
		public void ReName(string newName)
		{
			_name = newName;
			ChangeFullPath();
		}

		// Change the full path to the object, taking into account the presence of the parent object
		private void ChangeFullPath()
		{
			if (_parent != null)
				_fullPath = _parent._fullPath + $".{_name}";
			else
				_fullPath = $"{_name}";
		}

		#endregion

		#endregion

		#region ConsoleOutput

		public void Write()
		{
			Console.WriteLine($"\n Full path: {_fullPath}\n Level: {_level}\n Value: {_value}\n ValueType: {_valueType}\n");
			if (_childs==null || _childs.Count <= 0) return;
			foreach (var child in _childs) child.Write();
		}

		#endregion
	}
}