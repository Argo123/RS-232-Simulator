   M S B A M L   `   `   `  ����A  =RS-232, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:RS_232RS_232  5   	   "    RS_232.ConfigureModbusWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:RS_232     5   	     � Title$  ConfigureWindow��5   	   $	��450��6!   $��300.5��6.   .��5	      � .+�5
   
   �� -   5         ��Name$ label��5
      $��PORT:��6   $
��Left=�6/   $��	10,20,0,0q�6J   $	��Top=�6]   �� 5   
   -   5        $ label1��5      $��
Baud rate:��6    $
��Left=�65   $��	10,51,0,0q�6P   $	��Top=�6c   �� 5   
   -   5        $ label2��5      $��Sign format:��6    $
��Left=�67   $��	10,82,0,0q�6R   $	��Top=�6e   �� 5   
   -   5        $ label3��5      $��Station type:��6    $
��Left=�68   $��
10,113,0,0q�6S   $	��Top=�6g   �� 5   
   -   5        $ label4��5      $��Retransmission:��6    $
��Left=�6:   $��
10,144,0,0q�6U   $	��Top=�6i    J� RenderTransformOrigin$ -0.132,-0.769&�6�   �� 5   
   -   5        $ label5��5      $��Address:��6    $
��Left=�63   $��
10,175,0,0q�6N   $	��Top=�6b   �� 5   
   -   5         ��Name$ PortComboBox��5      $
��Left=�6)   $��
143,24,0,0q�6D   $	��Top=�6X   $	��120��6p   �� 5   
   -   5        $ SpeedComboBox��5      $
��Left=�6*   $��
143,55,0,0q�6E   $	��Top=�6Y   $	��120��6q   �� 5   
   -	   5        $ FormatComboBox��5      $
��Left=�6+   $��
143,86,0,0q�6F   $	��Top=�6Z   $	��120��6r   �� 5   
   -
   5        $ StationTypeComboBox��5      $
��Left=�60   $��143,117,0,0q�6K   $	��Top=�6`   $	��120��6x   6�   �� 5   
   -   5        $ RetransmissionComboBox��5      $
��Left=�63   $��143,148,0,0q�6N   $	��Top=�6c   $	��120��6{   �� 5   
   -   5         ��Name$ ConfigureButton��5      $��	Configure��6*   $
��Left=�6>   $��
90,228,0,0q�6Y   $	��Top=�6m   $��91��6�   $��44��6�   6�   �� 5   
   -   5        $ adressComboBox��5      $
��Left=�6+   $��143,179,0,0q�6F   $	��Top=�6[   $	��120��6s   5      5      