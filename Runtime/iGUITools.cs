using UnityEngine;
using System.IO;
using DateTime = System.DateTime;

namespace cmdwtf.UnityTools
{
#if IGUI_TOOLS
	public static class UnityTools
	{
		public static bool HasScrollPosition(this iGUI.iGUIElement ele)
		{
			if (ele is iGUIScrollView)
				return true;
			if (ele is iGUIListBox)
				return true;
			return false;
		}
		
		public static Vector2 GetScrollPosition(this iGUIElement ele)
		{
			if (ele is iGUIScrollView)
				return (ele as iGUIScrollView).scrollPosition;
			if (ele is iGUIListBox)
				return (ele as iGUIListBox).scrollPosition;
			return Vector2.zero;
		}
		
		public static bool TryRemoveElement(this iGUIElement ele, iGUIElement to_remove)
		{
			if (ele is iGUIContainer)
				return (ele as iGUIContainer).removeElement(to_remove);
			if (ele is iGUIScrollView)
				return (ele as iGUIScrollView).removeElement(to_remove);
			if (ele is iGUIListBox)
				return (ele as iGUIListBox).removeElement(to_remove);
			if (ele is iGUIWindow)
				return (ele as iGUIWindow).removeElement(to_remove);
			if (ele is iGUIPanel)
				return (ele as iGUIPanel).removeElement(to_remove);
		
			Debug.LogWarning("tryRemoveElement couldn't remove... type not supported? (" + ele.GetType().Name + ")");
			return false;
		}
		
		public static T FindComponentInHierarchy<T>(this iGUIElement ele, bool check_children = false) where T : UnityEngine.Component
		{
			if (ele == null)
				return null;
			
			T ret = default(T);
		
			// try this object
			ret = ele.GetComponent<T>();
			
			if (ret != null)
				return ret;
			
			// try parents
			ret = ele.container.findComponentInHierarchy<T>(check_children);
			
			if (ret != null)
				return ret;
			
			// try children
			if (ret == null && check_children)
				ret = ele.GetComponentInChildren<T>();
			
			return ret;
		}
		
		#region Position Related
		
		public static void ToAbsolute(this iGUIElement element)
		{
			Rect absoluteSelf = element.rect;
			Rect containerRect = element.containerRect;
			
			absoluteSelf.x -= containerRect.x;
			absoluteSelf.y -= containerRect.y;
			
			element.setPositionAndSize(absoluteSelf);
		}
		
		public static void ToRelative(this iGUIElement element)
		{
			Rect relativeSelf = new Rect();
			Rect containerRect = element.containerRect;
			
			relativeSelf.width = element.rect.width / containerRect.width;
			relativeSelf.height = element.rect.height / containerRect.height;
			
			if ((element.rect.x == 0 && containerRect.x == 0) || (element.rect.x-containerRect.x == 0) || (containerRect.width-element.rect.width == 0))
				relativeSelf.x = 0;
			else
				relativeSelf.x = (element.rect.x-containerRect.x) / (containerRect.width-element.rect.width);
			
			if ((element.rect.y == 0 && containerRect.y == 0) || (element.rect.y-containerRect.y == 0) || (containerRect.height-element.rect.height == 0))
				relativeSelf.y = 0;
			else
				relativeSelf.y = (element.rect.y-containerRect.y) / (containerRect.height-element.rect.height);

			if (relativeSelf.x >= 2)
				return;
			//element.label.text = "" + relativeSelf.x + "|" +containerRect.width + "|" + containerRect.width + "|" + element.rect.x + "|" + element.rect.width;

			//if (relativeSelf.x  - containerRect.x > 2)
			//	return;

			element.setPositionAndSize(relativeSelf);
		}
		
		#endregion Position Related

		public static void SetAllStyleBackgrounds(this iGUI.iGUIButton but, Texture2D tex)
		{
			but.style.normal.background = tex;
			but.style.active.background = tex;
			but.style.hover.background = tex;
		}
		public static bool IsClickable(this iGUI.iGUIElement button)
		{
			if (button == null)
			{
				Debug.LogWarning("IsClickable called with null!");
				return false;
			}

			if (button.passive) return false;
			if (button.enabled == false) return false;

			if (button.transform.parent != null)
			{
				iGUI.iGUIElement parent_ele = button.transform.parent.GetComponent<iGUI.iGUIElement>();
				if (parent_ele != null && parent_ele.IsClickable() == false)
					return false;
			}

			return true;
		}

		public static bool IsClickableNoParentCheck(this iGUI.iGUIElement button)
		{
			if (button == null) return false;
			if (button.passive) return false;
			if (button.enabled == false) return false;

			return true;
		}

		public static bool IsEnabledParents(this iGUI.iGUIElement ele)
		{
			if (ele == null)
			{
				Debug.LogWarning("IsClickable called with null!");
				return false;
			}

			if (ele.passive) return false;
			if (ele.enabled == false) return false;

			if (ele.transform.parent != null)
			{
				iGUI.iGUIElement parent_ele = ele.transform.parent.GetComponent<iGUI.iGUIElement>();
				if (parent_ele != null && parent_ele.IsEnabledParents() == false)
					return false;
			}

			return true;
		}

		public static bool IsMousePosIn(this iGUI.iGUIElement ele, Vector2 pos)
		{
			Vector2 pos_in_space = new Vector2(pos.x, Screen.height - pos.y);
			if (ele.getAbsoluteRect().Contains(pos_in_space))
				return true;
			return false;
		}
	}
#endif // IGUI_TOOLS
}