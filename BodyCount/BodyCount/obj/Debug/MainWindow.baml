   M S B A M L   `   `   `  ����D  @BodyCount, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:BodyCount	BodyCount  5   	   F BWPFMediaKit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullfBclr-namespace:WPFMediaKit.DirectShow.Controls;assembly=WPFMediaKitWPFMediaKit.DirectShow.Controls 5   	       BodyCount.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   )localclr-namespace:BodyCount     5   	   WcontrolsBclr-namespace:WPFMediaKit.DirectShow.Controls;assembly=WPFMediaKit     5   	     � Title$  
MainWindow��5   	   $
��1000��6   $
��1080��6*    � 	Resources 5      ��%    5
      .��5      � .+�5   
   �� -   5        $��textBox1��5   '   $y�Visible=�6   $��72(�67   $
��Left=�6E   $	��Top=�6`   $��824,87.233,0,0q�6x   $	��170��6�   6�   �� 5   
   -   5         ��Name$ listBox��5      $��824,194,10,0q�6#   $	��238��6:   ��5      �� 5      .R�5      �� $��border��6   
����� �$��1q�6:    �� Padding$ 5q�6N   $��5q�6Z   ._�5      �  � RowDefinitions 5      �� 5      �� 5      �� 5      6.   �� 5      6.   �� 5      6.    � ColumnDefinitions 5      �� 5      $	��100 �6/   �� 5      $	��180 �6/   .+�5      �� $��Right=�6$   $��0��6@   $��0��6M   $��TrackingID:��6]   �� 5       $��Right=�6$   $��1��6@   $��0��6M   $��Start Time:��6]   �� 5!      $��Right=�6$   $��2��6@   $��0��6M   $��Total Stay Time:��6]   6w   �� 5"      $��Right=�6$   $��3��6@   $��0��6M   $��	End Time:��6]   6p   �� 5#      $��Right=�6$   $��4��6@   $��0��6M   $��Dwell Time:��6]   6r   �� 5%      $��0��6$   $��1��61   ��6A   ��  �� Path$ 
TrackingID��� 5&      $��1��6$   $��1��61   ��6A   �� $ 	StartTime��� 5'      $��2��6$   $��1��61   ��6A   �� $
 Time��� 5(      $��3��6$   $��1��61   ��6A   �� $ EndTime��� 5)      $��4��6$   $��1��61   ��6A   �� $ 
DewellTime�5+      5,      5-      5/      �� 50      -   5         ��Name$ 
colorImage��50      $	��480��6    $	��640��6-    �� Left$ 10��69   
	 �� Top$
	 -312��6J   $��75,10,357,480q�6\   6w   �� 51   
   $	��480��6   $	��640��6   $��75,480,357,10q�6*   ._�52      �� -   5        
 ��Name$
 imageCanvas��52      .+�53      �� .+�54      9  3WPFMediaKit.DirectShow.Controls.VideoCaptureElement  -   5          Name$ videoElement��54   /   
   FPS$ 30��6E      DesiredPixelWidth$
 1080��6N      DesiredPixelHeight$	 720��6g   $	��480��55   /   $	��640��6<   6  0WPFMediaKit.DirectShow.Controls.MediaElementBase   LoadedBehavior$
 Play=�6H   1  +WPFMediaKit.DirectShow.Controls.D3DRenderer   Stretch$ Uniform=�6^   6s   56      �� 57      -   5        $
 bodyContainer��57      $	��640��6%   $	��480��61   6B   58      59      5>      5?      