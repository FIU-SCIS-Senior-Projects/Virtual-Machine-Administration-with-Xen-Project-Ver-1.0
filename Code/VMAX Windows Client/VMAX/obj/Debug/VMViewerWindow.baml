   M S B A M L   `   `   `  ����F  BVncSharpWpf, Version=0.0.0.1, Culture=neutral, PublicKeyToken=null>.clr-namespace:VncSharpWpf;assembly=VncSharpWpfVncSharpWpf  5   	   ? ;VMAX, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:VMAXVMAX 5   	      VMAX.VMViewerWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Aviewer.clr-namespace:VncSharpWpf;assembly=VncSharpWpf     5   	   $localclr-namespace:VMAX     5   	     � WindowStartupLocation$  CenterScreen=�5   	    � Title$ VMViewerWindow��5
   	   $
��1000��6    $
��1100��6.   .��5      � ���	#FFF8F8F8 � ColumnDefinitions 5   
   �� 5      $��1* �6    � RowDefinitions 5   
   �� 5      $
��1.5* �6   �� 5      $	��25* �6   �� 5      $��1* �6   .+�5   
   �� $��0��6   $��0��6   $��2��6.   $��1,0,0,2q�6B    J� Opacity$	 0.5Q�6\   
���   ���5      �� 5       �� EndPoint$
 .2,1&�6&    �� 
StartPoint$ 1.0,0&�66   .��5      �  � Color$ 	#FF525B7E��6#    � Offset$ 0.003Q�65   � 5      $ White��6#   $ 1Q�61   5      5      �� 5   
   -   5        	 ��Name$	 label_title��5      $��Virtual Machine��6%   $��Center=�6?   $��Center=�6Z   $��0��6w   $��2��6�   $��Aharoni)�6�   $��24(�6�   ���	#FF4242426�   "   VncSharpWpf.RemoteDesktopWpf  5   
   -   5        
  Name$
 viewer��5   "   $��1��62   $��1��6B   $��0,23,0,0q�6O   �� 5   
   -   5         ��Name$ border_status��5      $	��Top=�6(   $��1��6@   ���	#FF427CD1$��23��6d   ._�5      �� -   5         ��Name$ textblock_status��5   C   $��Status��6   $��Center=�6&   ���	#FFF7E5E5$
��Bold"�6t   5       5!      5"      