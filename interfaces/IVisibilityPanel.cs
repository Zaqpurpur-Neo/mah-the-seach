using System.Windows;

namespace nah_the_search.interfaces;

public interface IVisibilityPanel {
	public string ViewName { get; }
	public Visibility _panelVisibility { get; set; }
	public Visibility PanelVisibility { get; set; }

	public void HidePanel();
	public void ShowPanel();
}
