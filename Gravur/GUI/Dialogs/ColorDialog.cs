//==========================================================================================
//
//		OpenNETCF.Windows.Forms.ColorDialog
//		Copyright (C) 2003-2004, OpenNETCF.org
//
//		This library is free software; you can redistribute it and/or modify it under 
//		the terms of the OpenNETCF.org Shared Source License.
//
//		This library is distributed in the hope that it will be useful, but 
//		WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
//		FITNESS FOR A PARTICULAR PURPOSE. See the OpenNETCF.org Shared Source License 
//		for more details.
//
//		You should have received a copy of the OpenNETCF.org Shared Source License 
//		along with this library; if not, email licensing@opennetcf.org to request a copy.
//
//		If you wish to contact the OpenNETCF Advisory Board to discuss licensing, please 
//		email licensing@opennetcf.org.
//
//		For general enquiries, email enquiries@opennetcf.org or visit our website at:
//		http://www.opennetcf.org
//
//==========================================================================================
using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
//using OpenNETCF.Drawing;

#if DESIGN && STANDALONE
[assembly: System.CF.Design.RuntimeAssemblyAttribute("OpenNETCF.Windows.Forms.ColorDialog, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
#endif

namespace GravurGIS.GUI.Dialogs
{
	/// <summary>
	/// Represents a common dialog box that displays available colors along with controls that allow the user to define custom colors.
	/// </summary>
	/// <remarks>The inherited member <see cref="ShowDialog"/> must be invoked to create this specific common dialog box.
	/// Use <see cref="Color"/> to retrieve the color selected by the user.
	/// <para>When you create an instance of ColorDialog, some of the read/write properties are set to initial values.
	/// For a list of these values, see the ColorDialog constructor.</para></remarks>
	/// <platform><frameworks><compact>true</compact></frameworks></platform>
	public sealed class ColorDialog : System.Windows.Forms.CommonDialog
	{
		//properties used to invoke the dialog
		private CHOOSECOLOR m_color;

		//bgr values of custom colors
		private int[] m_rawcustom;
		//gc handle to the above array
		private GCHandle m_custhandle;

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <b>ColorDialog</b> class.
		/// </summary>
		/// <remarks>When you create an instance of ColorDialog, the following read/write properties are set to initial values.
		/// <list type="table"><listheader><term>Property</term><term>Initial Value</term></listheader>
		/// <item><term>AllowFullOpen</term><term>true</term></item> 
		/// <item><term>Color</term><term>Color.Black</term></item>
		///	<item><term>CustomColors</term><term>A null reference (Nothing in Visual Basic)</term></item>
		/// <item><term>FullOpen</term><term>false</term></item></list>
		///
		/// <para>You can change the value for any of these properties through a separate call to the property.</para></remarks>
		public ColorDialog(IntPtr hwndHandle)
		{
			m_color = new CHOOSECOLOR();
            m_color.hwndOwner = hwndHandle;
			//array for custom colors
			m_rawcustom = new int[16];
			m_custhandle = GCHandle.Alloc(m_rawcustom, GCHandleType.Pinned);
			m_color.lpCustColors = m_custhandle.AddrOfPinnedObject();
		}
		#endregion

		#region Dispose
		/// <summary>
		/// Free up resources used by the ColorDialog
		/// </summary>
		public new void Dispose()
		{
			//free pinned array (if allocated)
			if(m_custhandle.IsAllocated)
			{
				m_custhandle.Free();
			}
			base.Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion

		
		#region Allow Full Open
		/// <summary>
		/// Gets or sets a value indicating whether the user can use the dialog box to define custom colors.
		/// </summary>
		/// <value>true if the user can define custom colors; otherwise, false. The default is true</value>
		/// <remarks>When set to false, the associated button in the dialog box is disabled and the user cannot access the custom colors control in the dialog box.
		/// Windows CE supports the button, Pocket PC does not and this property has no effect.</remarks>
		/// <example>[Visual Basic, C#] The following example illustrates the creation of new ColorDialog.
		/// This example assumes that the method is called from within an existing form, that has a TextBox and Button placed on it.
		/// <code>[Visual Basic] 
		/// Protected Sub button1_Click(sender As Object, e As System.EventArgs)
		///			Dim MyDialog As New ColorDialog()
		///			' Keeps the user from selecting a custom color.
		///			MyDialog.AllowFullOpen = False
		///			' Sets the initial color select to the current text color,
		///			MyDialog.Color = textBox1.ForeColor
		///			
		///			' Update the text box color if the user clicks OK 
		///			If (MyDialog.ShowDialog() = DialogResult.OK) Then
		///					textBox1.ForeColor =  MyDialog.Color
		///			End If
		///
		///	End Sub 'button1_Click</code>
		///	<code>[C#] 
		///protected void button1_Click(object sender, System.EventArgs e)
		///{
		///		ColorDialog MyDialog = new ColorDialog();
		///		// Keeps the user from selecting a custom color.
		///		MyDialog.AllowFullOpen = false ;
		///		// Sets the initial color select to the current text color.
		///		MyDialog.Color = textBox1.ForeColor ;
		///
		///		// Update the text box color if the user clicks OK 
		///		if (MyDialog.ShowDialog() == DialogResult.OK)
		///			textBox1.ForeColor =  MyDialog.Color;
		///}</code>
		///</example>
		[DefaultValue(true)]
		public bool AllowFullOpen
		{
			get
			{
				return (m_color.Flags & (uint)ColorFlags.PreventFullOpen) == 0 ? true : false;
			}
			set
			{
				if(value)
				{
					m_color.Flags = m_color.Flags & ~(uint)ColorFlags.PreventFullOpen;
				}
				else
				{
					m_color.Flags = m_color.Flags | (uint)ColorFlags.PreventFullOpen;
				}
			}
		}
		#endregion

		#region Any Color
		/// <summary>
		/// Gets or sets a value indicating whether the dialog box displays all available colors in the set of basic colors.
		/// </summary>
		/// <value>true if the dialog box displays all available colors in the set of basic colors; otherwise, false.
		/// The default value is false.</value>
		[DefaultValue(false)]
		public bool AnyColor
		{
			get
			{
				return (m_color.Flags & (uint)ColorFlags.AnyColor) == 0 ? false : true;
			}
			set
			{
				if(value)
				{
					m_color.Flags = m_color.Flags | (uint)ColorFlags.AnyColor;
				}
				else
				{
					m_color.Flags = m_color.Flags & ~(uint)ColorFlags.AnyColor;
				}
			}
		}
		#endregion

		#region Color
		/// <summary>
		/// Gets or sets the color selected by the user.
		/// </summary>
		/// <value>The color selected by the user.
		/// If a color is not selected, the default value is black.</value>
		/// <remarks>The color selected by the user in the dialog box at run time, as defined in <see cref="T:System.Drawing.Color"/> structure</remarks>
		/// <example>[Visual Basic, C#] The following example illustrates the creation of new ColorDialog.
		/// This example assumes that the method is called from within an existing form, that has a TextBox and Button placed on it.
		/// <code>[Visual Basic] 
		/// Protected Sub button1_Click(sender As Object, e As System.EventArgs)
		///			Dim MyDialog As New ColorDialog()
		///			' Keeps the user from selecting a custom color.
		///			MyDialog.AllowFullOpen = False
		///			' Sets the initial color select to the current text color,
		///			MyDialog.Color = textBox1.ForeColor
		///			
		///			' Update the text box color if the user clicks OK 
		///			If (MyDialog.ShowDialog() = DialogResult.OK) Then
		///					textBox1.ForeColor =  MyDialog.Color
		///			End If
		///
		///	End Sub 'button1_Click</code>
		///	<code>[C#] 
		///protected void button1_Click(object sender, System.EventArgs e)
		///{
		///		ColorDialog MyDialog = new ColorDialog();
		///		// Keeps the user from selecting a custom color.
		///		MyDialog.AllowFullOpen = false ;
		///		// Sets the initial color select to the current text color.
		///		MyDialog.Color = textBox1.ForeColor ;
		///
		///		// Update the text box color if the user clicks OK 
		///		if (MyDialog.ShowDialog() == DialogResult.OK)
		///			textBox1.ForeColor =  MyDialog.Color;
		///}</code>
		///</example>
		public System.Drawing.Color Color
		{
			get
			{
				//return System.Drawing.Color.FromArgb(BGRToARGB(m_color.rgbResult));
				return ColorTranslator.FromWin32(m_color.rgbResult);
			}
			set
			{
				//m_color.rgbResult = ARGBToBGR(value.ToArgb());
				m_color.rgbResult = ColorTranslator.ToWin32(value);
				
                //set flag that starting value is supplied
				m_color.Flags = m_color.Flags | (uint)ColorFlags.RgbInit;
			}
		}
		#endregion

		#region Custom Colors
		/// <summary>
		/// Gets or sets the set of custom colors shown in the dialog box.
		/// </summary>
		/// <value>A set of custom colors shown by the dialog box.
		/// The default value is a null reference (Nothing in Visual Basic).</value>
		/// <remarks>Users can create their own set of custom colors.
		/// These colors are contained in an Int32 composed of the ARGB component (alpha, red, green, and blue) values necessary to create the color.
		/// For more information on the structure of this data, see <see cref="T:System.Drawing.Color"/>.
		/// Custom colors can only be defined if <see cref="P:OpenNETCF.Windows.Forms.ColorDialog.AllowFullOpen"/> is set to true (Not supported on Pocket PC).</remarks>
		/// <example>[Visual Basic, C#] The following example shows how to add an array of type Int32 representing custom colors to CustomColors.
		/// This example assumes that the code is run from within a Form.
		/// <code>[Visual Basic] 
		/// Dim MyDialog = New ColorDialog()
		/// 'Allows the user to select or edit a custom color.
		/// MyDialog.AllowFullOpen = True
		/// 'Assigns an array of custom colors to the CustomColors property.
		/// MyDialog.CustomColors = New Integer() {6916092, 15195440, 16107657, 1836924, _
		///		3758726, 12566463, 7526079, 7405793, 6945974, 241502, 2296476, 5130294, _
		///		3102017, 7324121, 14993507, 11730944}
		///	'Sets the initial color select to the current text color,
		///	'so that if the user cancels out, the original color is restored.
		///	MyDialog.Color = Me.BackColor
		///	MyDialog.ShowDialog()
		///	Me.BackColor = MyDialog.Color</code>
		///	<code>[C#] 
		///	System.Windows.Forms.ColorDialog MyDialog = new ColorDialog();
		/// // Allows the user to select or edit a custom color.
		/// MyDialog.AllowFullOpen = true ;
		/// // Assigns an array of custom colors to the CustomColors property
		/// MyDialog.CustomColors = new int[] {6916092, 15195440, 16107657, 1836924,
		///		3758726, 12566463, 7526079, 7405793, 6945974, 241502, 2296476, 5130294,
		///		3102017, 7324121, 14993507, 11730944,};
		/// // Sets the initial color select to the current text color,
		/// // so that if the user cancels out, the original color is restored.
		/// MyDialog.Color = this.BackColor;
		/// MyDialog.ShowDialog();
		/// this.BackColor =  MyDialog.Color;</code>
		/// </example>
		public int[] CustomColors
		{
			get
			{
				int[] custom = new int[16];

				//get native colors
				for(int iColor = 0; iColor < 16; iColor++)
				{
					custom[iColor] = ColorTranslator.FromWin32(m_rawcustom[iColor]).ToArgb();//BGRToARGB(m_rawcustom[iColor]);
				}

				//return managed colors
				return custom;
			}
			set
			{
				for(int iCount = 0; iCount <  16; iCount++)
				{
					if(iCount >= value.Length)
					{
						m_rawcustom[iCount] = ColorTranslator.ToWin32(System.Drawing.Color.Black);
					}
					else
					{
						m_rawcustom[iCount] = ColorTranslator.ToWin32(System.Drawing.Color.FromArgb(value[iCount]));
					}
				}

				//free current handle (if allocated)
				if(m_custhandle.IsAllocated)
				{
					m_custhandle.Free();
				}
				//pin the array
				m_custhandle = GCHandle.Alloc(m_rawcustom, GCHandleType.Pinned);
				//offset first value which is blank
				m_color.lpCustColors = (IntPtr)((int)m_custhandle.AddrOfPinnedObject()+4);
			}
		}
		#endregion

		#region Full Open
		/// <summary>
		/// Gets or sets a value indicating whether the controls used to create custom colors are visible when the dialog box is opened.
		/// </summary>
		/// <value>true if the custom color controls are available when the dialog box is opened; otherwise, false.
		/// The default value is false.</value>
		/// <remarks>By default, the custom color controls are not visible when the dialog box is first opened.
		/// You must click the Custom Colors button to display them.
		/// <para>Note   If <see cref="P:OpenNETCF.Windows.Forms.ColorDialog.AllowFullOpen"/> is false, then <b>FullOpen</b> has no effect.</para></remarks>
		[DefaultValue(false)]
		public bool FullOpen
		{
			get
			{
				return (m_color.Flags & (uint)ColorFlags.FullOpen) == 0 ? false : true;
			}
			set
			{
				if(value)
				{
					//dissallow if allowfull open is false
					if(this.AllowFullOpen==true)
					{
						m_color.Flags = m_color.Flags | (uint)ColorFlags.FullOpen;
					}
					else
					{
						this.FullOpen=false;
					}
				}
				else
				{
					m_color.Flags = m_color.Flags & ~(uint)ColorFlags.FullOpen;
				}
			}
		}
		#endregion

		#region Show Dialog
		/// <summary>
		/// Runs a common dialog box.
		/// </summary>
		/// <returns>DialogResult.OK if the user clicks <b>OK</b> in the dialog box; otherwise, DialogResult.Cancel.</returns>
		/// <example>The following example uses the ColorDialog and illustrates creating and showing a dialog box.
		/// This example assumes that the method is called from within an existing form, that has a TextBox and Button placed on it.
		/// <para>Note:   This example shows how to use one of the overloaded versions of ShowDialog.
		/// For other examples that might be available, see the individual overload topics.</para>
		/// <code>[C#]
		/// protected void button1_Click(object sender, System.EventArgs e)
		/// {
		///		ColorDialog MyDialog = new ColorDialog();
		///		// Keeps the user from selecting a custom color.
		///		MyDialog.AllowFullOpen = false ;
		///		// Sets the initial color select to the current text color.
		///		MyDialog.Color = textBox1.ForeColor ;
		///		// Update the text box color if the user clicks OK 
		///		if (MyDialog.ShowDialog() == DialogResult.OK)
		///			textBox1.ForeColor =  MyDialog.Color;
		/// }
		/// </code></example>
        public new DialogResult ShowDialog()
		{
			//show dialog with current focussed window as owner
            bool success = RunDialog();

			if(success==true)
			{
				return DialogResult.OK;
			}
			else
			{
				int error = CommDlgExtendedError();
				return DialogResult.Cancel;
			}
		}

#if !DESIGN
		/// <summary>
		/// Specifies a common dialog box.
		/// </summary>
		/// <param name="hwndOwner">A value that represents the window handle of the owner window for the common dialog box.</param>
		/// <returns>true if the dialog box was successfully run; otherwise, false.</returns>
		/// <remarks>This method is invoked when the user of a common dialog box calls <see cref="ShowDialog"/>.</remarks>
		//internal bool RunDialog(IntPtr hwndOwner)
        internal bool RunDialog()
		{

            //Change: The handle is already set in constructor
			//set handle in structure
			//m_color.hwndOwner = hwndOwner;

			//call native function
			return ChooseColor(m_color);
		}
#endif
		#endregion

		#region Solid Color Only
		/// <summary>
		/// Gets or sets a value indicating whether the dialog box will restrict users to selecting solid colors only.
		/// </summary>
		[DefaultValue(false)]
		public bool SolidColorOnly
		{
			get
			{
				return (m_color.Flags & (uint)ColorFlags.SolidColor) == 0 ? false : true;
			}
			set
			{
				if(value)
				{
					m_color.Flags = m_color.Flags | (uint)ColorFlags.SolidColor;
				}
				else
				{
					m_color.Flags = m_color.Flags & ~(uint)ColorFlags.SolidColor;
				}

				Console.WriteLine(m_color.Flags.ToString("X"));
			}
		}
		#endregion

		#region API

		[DllImport("commdlg.dll")]
		private static extern bool ChooseColor(CHOOSECOLOR lpcc);

		[DllImport("commdlg.dll")]
		private static extern int CommDlgExtendedError();

		#endregion

		#region Choose Color Structure

		[StructLayout(LayoutKind.Sequential)]
		private class CHOOSECOLOR
		{
			private int lStructSize;
			internal IntPtr hwndOwner;
			IntPtr hInstance;
			internal int rgbResult;
			internal IntPtr lpCustColors;
			internal uint Flags;
			private int lCustData;
			private IntPtr lpfnHook;
			private IntPtr lpTemplateName;

			public CHOOSECOLOR()
			{
				lStructSize = Marshal.SizeOf(this);
				hwndOwner = IntPtr.Zero;
				hInstance = IntPtr.Zero;
				rgbResult = 0x00ffffff;
				lpCustColors = IntPtr.Zero;	
				Flags = 0;
				lCustData = 0;
				lpfnHook = IntPtr.Zero;
				lpTemplateName = IntPtr.Zero;
			}

			#region IDisposable Members

			#endregion
		}
		#endregion

		
		#region Color Flags Enumeration
		[Flags()]
		private enum ColorFlags : uint
		{
			RgbInit               =	0x00000001,
			FullOpen              =	0x00000002,
			PreventFullOpen       =	0x00000004,
			//ENABLEHOOK            =	0x00000010,
			//ENABLETEMPLATE        =	0x00000020,
			//ENABLETEMPLATEHANDLE  =	0x00000040,
			SolidColor            =	0x00000080,
			AnyColor              =	0x00000100
		}
		#endregion

#if DESIGN
		public override void Reset()
		{
			m_color.Flags = 0;
		}

		protected override bool RunDialog(IntPtr hWnd)
		{
			return true;
		}
#endif
	}

	
}
