   M S B A M L   `   `   `  ���� B  >GHJTool, Version=1.0.0.1, Culture=neutral, PublicKeyToken=null-clr-namespace:GHJTool.ViewsGHJTool.Views  &     GHJTool.Views.FilteringAglorithm   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     -localclr-namespace:GHJTool.Views     X TXceed.Wpf.Toolkit, Version=4.0.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xwtk)http://schemas.xceed.com/wpf/xaml/toolkit  .��� .+��� $��Center=�$��16,10q�.+��� $
��0,10q�$	��120��  �� MinLines$  1��$��Center=�$��Center=� �� TextAlignment$
 Left=����� *FilteringAlgorithmResultStr+��  �� Orientation$ 
Horizontal=�.+��� $��0,5,5,0q�$$��趋势线差值之和规格：��$��28��$	��200��$
��Left=�$��Center=�$  Xceed.Wpf.Toolkit.DoubleUpDown  $	��200��$��28���  �Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   Value �� *SubSumTrend+�� $ 
Horizontal=�.+��� $��0,5,5,0q�$��趋势线总和规格：��$��28��$	��200��$
��Left=�$��Center=�  $	��200��$��28�� �� *GlobalSumTrend+�� $ 
Horizontal=�.+��� $��0,5,5,0q�$*��$百分比差值能量阶跃规格：��$��28��$	��200��$
��Left=�$��Center=�  $	��200��$��28�� �� *EnergyStepPercentage+�� $ 
Horizontal=�.+��� $��0,5,5,0q�$$��百分比差值数量规格：��$��28��$	��200��$
��Left=�$��Center=�  $	��200��$��28�� �� *DValuePercentage+�� $ 
Horizontal=�.+��� $��0,5,5,0q�$-��'超百分比差值数量规格数量：��$��28��$	��200��$
��Left=�$��Center=�%  Xceed.Wpf.Toolkit.IntegerUpDown  $	��200��$��28���  �Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   Value �� *DValuePercentageNum+�� $ 
Horizontal=�.+��� -    ��Name$ cmbfilelist��$��0,0,5,0q�$��28��$	��200��$
��Left=�$��Center=��� -   $��28��$	��200��$��导入多个数据文件���� $ 
Horizontal=�.+��� -    ��Name$" btnFilteringAlgorithmProcess��$��
205,10,0,0q�$��28��$	��200��$��	Filtering��