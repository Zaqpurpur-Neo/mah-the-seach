using System.Windows;

namespace raycast_copy.interfaces;

public interface IVisibilityPanel {
	public string ViewName { get; }
	public Visibility _panelVisibility { get; set; }
	public Visibility PanelVisibility { get; set; }

	public void HidePanel();
	public void ShowPanel();
}
