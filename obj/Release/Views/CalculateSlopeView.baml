   M S B A M L   `   `   `  ���� B  >GHJTool, Version=1.0.0.1, Culture=neutral, PublicKeyToken=null-clr-namespace:GHJTool.ViewsGHJTool.Views  5clr-namespace:GHJTool.ViewModelGHJTool.ViewModel  &     GHJTool.Views.CalculateSlopeView   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     -localclr-namespace:GHJTool.Views     .vmclr-namespace:GHJTool.ViewModel     X TXceed.Wpf.Toolkit, Version=4.0.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xwtk)http://schemas.xceed.com/wpf/xaml/toolkit    � 	Resources     treeViewItemStyle_Expanded �� 
TargetType �� Property �� Value   treeViewItemStyle_UnExpanded��%l   &	        &	 ,     �� " Q�.���  �@Q�
IsExpanded . �� " Q�.���  �@Q�
IsExpanded .  .���  � RowDefinitions �� �� ��  � ColumnDefinitions �� �� .+��� $��0��$��2��$��0��$��10q�$
��Auto=�.��R� -    R�Name$ 	tvDataPos��$��8q�#��[ $��1q����� *TvNodeItems+����  �� DataType(   "GHJTool.ViewModel.TreeViewNodeItem"   �� ItemsSource �� *
Children+.R��� 	 �� Orientation$	 
Horizontal=�.+��� $
��Left=�$��Center=����� *	Content+�� -   
 ��Name$
 UiSlopeInfos��$��16,10q�$��1��$��2��.+��� $��Right=�$	 
Horizontal=�.+��� -    ��Name$ cmbfilelist��$��0,0,5,0q�$��28��$	��200��$
��Left=�$��Center=����� *FileNameList+ �� SelectedItem �� *
FileName+�� -   $��导入多个数据文件���� $y�	Collapsed=�$��0,2,0,0q�$��Right=�$	 
Horizontal=�.+��� $��0,0,20,0q�$��拟合阶数��$
��Left=�$��Center=�%  Xceed.Wpf.Toolkit.IntegerUpDown  $	��200��$��26��$��Center=�$��Center=�,  &Xceed.Wpf.Toolkit.Primitives.InputBase   TextAlignment$ Center=��  �Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   Minimum$ 2X�   Value �� *Node+�� $��Right=�$	 
Horizontal=�.+��� .+��� -    ��Name$ chbSaveResult��$��10(�$��0,8,5,0q�$
��Left=�$��Center=�$��Bottom=�$!��是否导出结果到文件�� d� 	IsChecked �� *IsExportResult2Csv+�� -   $ chbSaveResult2SingleFile��$��10(�$��0,8,5,0q�$
��Left=�$��Center=�$	��Top=�$!��是否导出到同一文件�� �� *IsExportResult2SameCsv+�� -   $��	10,10,0,0q�$��28��$��计 算 Slope���� $��10q�$��1��$��2��$��0��$
��Auto=�.��R� -   $ tvPeak��$��8q�#��[  $��1q����� *TvNodeItems+���� "   �� *
Children+.R��� $	 
Horizontal=�.+��� $
��Left=�$��Center=����� *	Content+�� $��18q�$��0q�$��日志显示��$��0��$��2��.��#  Xceed.Wpf.Toolkit.RichTextBox  -	     Name$ txtLog��$
��Auto=�$	��100��$��0q� �/� ��.  /� 
LineHeight$ 5��