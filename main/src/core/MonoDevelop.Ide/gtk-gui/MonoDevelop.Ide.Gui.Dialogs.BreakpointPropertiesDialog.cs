// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace MonoDevelop.Ide.Gui.Dialogs {
    
    
    public partial class BreakpointPropertiesDialog {
        
        private Gtk.VBox vbox2;
        
        private Gtk.Label label1;
        
        private Gtk.Table table1;
        
        private Gtk.Entry entryFile;
        
        private Gtk.HBox hbox1;
        
        private Gtk.Entry entryLine;
        
        private Gtk.Label label4;
        
        private Gtk.Label label5;
        
        private Gtk.Label label6;
        
        private Gtk.Label label2;
        
        private Gtk.HBox hbox4;
        
        private Gtk.Label label14;
        
        private Gtk.VBox vbox4;
        
        private Gtk.RadioButton radioBreakAlways;
        
        private Gtk.RadioButton radioBreakTrue;
        
        private Gtk.RadioButton radioBreakChange;
        
        private Gtk.HBox boxCondition;
        
        private Gtk.Label label15;
        
        private Gtk.Label label7;
        
        private Gtk.Entry entryCondition;
        
        private Gtk.HSeparator hseparator1;
        
        private Gtk.HBox hbox7;
        
        private Gtk.Label label8;
        
        private Gtk.SpinButton spinHitCount;
        
        private Gtk.Label label3;
        
        private Gtk.HBox hbox2;
        
        private Gtk.Label label10;
        
        private Gtk.VBox vbox3;
        
        private Gtk.Label label11;
        
        private Gtk.RadioButton radioActionBreak;
        
        private Gtk.RadioButton radioActionTrace;
        
        private Gtk.HBox boxTraceExpression;
        
        private Gtk.Label label13;
        
        private Gtk.Label label12;
        
        private Gtk.Entry entryTraceExpr;
        
        private Gtk.Button buttonCancel;
        
        private Gtk.Button buttonOk;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize(this);
            // Widget MonoDevelop.Ide.Gui.Dialogs.BreakpointPropertiesDialog
            this.Name = "MonoDevelop.Ide.Gui.Dialogs.BreakpointPropertiesDialog";
            this.Title = Mono.Unix.Catalog.GetString("Breakpoint Properties");
            this.WindowPosition = ((Gtk.WindowPosition)(4));
            this.Modal = true;
            // Internal child MonoDevelop.Ide.Gui.Dialogs.BreakpointPropertiesDialog.VBox
            Gtk.VBox w1 = this.VBox;
            w1.Name = "dialog1_VBox";
            w1.BorderWidth = ((uint)(2));
            // Container child dialog1_VBox.Gtk.Box+BoxChild
            this.vbox2 = new Gtk.VBox();
            this.vbox2.Name = "vbox2";
            this.vbox2.Spacing = 6;
            this.vbox2.BorderWidth = ((uint)(9));
            // Container child vbox2.Gtk.Box+BoxChild
            this.label1 = new Gtk.Label();
            this.label1.Name = "label1";
            this.label1.Xalign = 0F;
            this.label1.LabelProp = Mono.Unix.Catalog.GetString("<b>Location</b>");
            this.label1.UseMarkup = true;
            this.vbox2.Add(this.label1);
            Gtk.Box.BoxChild w2 = ((Gtk.Box.BoxChild)(this.vbox2[this.label1]));
            w2.Position = 0;
            w2.Expand = false;
            w2.Fill = false;
            // Container child vbox2.Gtk.Box+BoxChild
            this.table1 = new Gtk.Table(((uint)(2)), ((uint)(3)), false);
            this.table1.Name = "table1";
            this.table1.RowSpacing = ((uint)(6));
            this.table1.ColumnSpacing = ((uint)(6));
            // Container child table1.Gtk.Table+TableChild
            this.entryFile = new Gtk.Entry();
            this.entryFile.CanFocus = true;
            this.entryFile.Name = "entryFile";
            this.entryFile.IsEditable = true;
            this.entryFile.InvisibleChar = '●';
            this.table1.Add(this.entryFile);
            Gtk.Table.TableChild w3 = ((Gtk.Table.TableChild)(this.table1[this.entryFile]));
            w3.LeftAttach = ((uint)(2));
            w3.RightAttach = ((uint)(3));
            w3.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.hbox1 = new Gtk.HBox();
            this.hbox1.Name = "hbox1";
            this.hbox1.Spacing = 6;
            // Container child hbox1.Gtk.Box+BoxChild
            this.entryLine = new Gtk.Entry();
            this.entryLine.CanFocus = true;
            this.entryLine.Name = "entryLine";
            this.entryLine.IsEditable = true;
            this.entryLine.WidthChars = 10;
            this.entryLine.InvisibleChar = '●';
            this.hbox1.Add(this.entryLine);
            Gtk.Box.BoxChild w4 = ((Gtk.Box.BoxChild)(this.hbox1[this.entryLine]));
            w4.Position = 0;
            this.table1.Add(this.hbox1);
            Gtk.Table.TableChild w5 = ((Gtk.Table.TableChild)(this.table1[this.hbox1]));
            w5.TopAttach = ((uint)(1));
            w5.BottomAttach = ((uint)(2));
            w5.LeftAttach = ((uint)(2));
            w5.RightAttach = ((uint)(3));
            w5.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.label4 = new Gtk.Label();
            this.label4.Name = "label4";
            this.label4.Xalign = 0F;
            this.label4.LabelProp = Mono.Unix.Catalog.GetString("File:");
            this.table1.Add(this.label4);
            Gtk.Table.TableChild w6 = ((Gtk.Table.TableChild)(this.table1[this.label4]));
            w6.LeftAttach = ((uint)(1));
            w6.RightAttach = ((uint)(2));
            w6.XOptions = ((Gtk.AttachOptions)(4));
            w6.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.label5 = new Gtk.Label();
            this.label5.Name = "label5";
            this.label5.LabelProp = Mono.Unix.Catalog.GetString("Line:");
            this.table1.Add(this.label5);
            Gtk.Table.TableChild w7 = ((Gtk.Table.TableChild)(this.table1[this.label5]));
            w7.TopAttach = ((uint)(1));
            w7.BottomAttach = ((uint)(2));
            w7.LeftAttach = ((uint)(1));
            w7.RightAttach = ((uint)(2));
            w7.XOptions = ((Gtk.AttachOptions)(4));
            w7.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.label6 = new Gtk.Label();
            this.label6.WidthRequest = 12;
            this.label6.Name = "label6";
            this.table1.Add(this.label6);
            Gtk.Table.TableChild w8 = ((Gtk.Table.TableChild)(this.table1[this.label6]));
            w8.XOptions = ((Gtk.AttachOptions)(4));
            w8.YOptions = ((Gtk.AttachOptions)(4));
            this.vbox2.Add(this.table1);
            Gtk.Box.BoxChild w9 = ((Gtk.Box.BoxChild)(this.vbox2[this.table1]));
            w9.Position = 1;
            w9.Expand = false;
            w9.Fill = false;
            // Container child vbox2.Gtk.Box+BoxChild
            this.label2 = new Gtk.Label();
            this.label2.Name = "label2";
            this.label2.Xalign = 0F;
            this.label2.LabelProp = Mono.Unix.Catalog.GetString("<b>Condition</b>");
            this.label2.UseMarkup = true;
            this.vbox2.Add(this.label2);
            Gtk.Box.BoxChild w10 = ((Gtk.Box.BoxChild)(this.vbox2[this.label2]));
            w10.Position = 2;
            w10.Expand = false;
            w10.Fill = false;
            // Container child vbox2.Gtk.Box+BoxChild
            this.hbox4 = new Gtk.HBox();
            this.hbox4.Name = "hbox4";
            this.hbox4.Spacing = 6;
            // Container child hbox4.Gtk.Box+BoxChild
            this.label14 = new Gtk.Label();
            this.label14.WidthRequest = 12;
            this.label14.Name = "label14";
            this.hbox4.Add(this.label14);
            Gtk.Box.BoxChild w11 = ((Gtk.Box.BoxChild)(this.hbox4[this.label14]));
            w11.Position = 0;
            w11.Expand = false;
            w11.Fill = false;
            // Container child hbox4.Gtk.Box+BoxChild
            this.vbox4 = new Gtk.VBox();
            this.vbox4.Name = "vbox4";
            this.vbox4.Spacing = 6;
            // Container child vbox4.Gtk.Box+BoxChild
            this.radioBreakAlways = new Gtk.RadioButton(Mono.Unix.Catalog.GetString("Always break"));
            this.radioBreakAlways.CanFocus = true;
            this.radioBreakAlways.Name = "radioBreakAlways";
            this.radioBreakAlways.DrawIndicator = true;
            this.radioBreakAlways.UseUnderline = true;
            this.radioBreakAlways.Group = new GLib.SList(System.IntPtr.Zero);
            this.vbox4.Add(this.radioBreakAlways);
            Gtk.Box.BoxChild w12 = ((Gtk.Box.BoxChild)(this.vbox4[this.radioBreakAlways]));
            w12.Position = 0;
            w12.Expand = false;
            w12.Fill = false;
            // Container child vbox4.Gtk.Box+BoxChild
            this.radioBreakTrue = new Gtk.RadioButton(Mono.Unix.Catalog.GetString("Break when condition is true"));
            this.radioBreakTrue.CanFocus = true;
            this.radioBreakTrue.Name = "radioBreakTrue";
            this.radioBreakTrue.DrawIndicator = true;
            this.radioBreakTrue.UseUnderline = true;
            this.radioBreakTrue.Group = this.radioBreakAlways.Group;
            this.vbox4.Add(this.radioBreakTrue);
            Gtk.Box.BoxChild w13 = ((Gtk.Box.BoxChild)(this.vbox4[this.radioBreakTrue]));
            w13.Position = 1;
            w13.Expand = false;
            w13.Fill = false;
            // Container child vbox4.Gtk.Box+BoxChild
            this.radioBreakChange = new Gtk.RadioButton(Mono.Unix.Catalog.GetString("Break when the expression changes"));
            this.radioBreakChange.CanFocus = true;
            this.radioBreakChange.Name = "radioBreakChange";
            this.radioBreakChange.DrawIndicator = true;
            this.radioBreakChange.UseUnderline = true;
            this.radioBreakChange.Group = this.radioBreakAlways.Group;
            this.vbox4.Add(this.radioBreakChange);
            Gtk.Box.BoxChild w14 = ((Gtk.Box.BoxChild)(this.vbox4[this.radioBreakChange]));
            w14.Position = 2;
            w14.Expand = false;
            w14.Fill = false;
            // Container child vbox4.Gtk.Box+BoxChild
            this.boxCondition = new Gtk.HBox();
            this.boxCondition.Name = "boxCondition";
            this.boxCondition.Spacing = 6;
            // Container child boxCondition.Gtk.Box+BoxChild
            this.label15 = new Gtk.Label();
            this.label15.WidthRequest = 12;
            this.label15.Name = "label15";
            this.boxCondition.Add(this.label15);
            Gtk.Box.BoxChild w15 = ((Gtk.Box.BoxChild)(this.boxCondition[this.label15]));
            w15.Position = 0;
            w15.Expand = false;
            w15.Fill = false;
            // Container child boxCondition.Gtk.Box+BoxChild
            this.label7 = new Gtk.Label();
            this.label7.Name = "label7";
            this.label7.Xalign = 0F;
            this.label7.LabelProp = Mono.Unix.Catalog.GetString("Condition expression:");
            this.boxCondition.Add(this.label7);
            Gtk.Box.BoxChild w16 = ((Gtk.Box.BoxChild)(this.boxCondition[this.label7]));
            w16.Position = 1;
            w16.Expand = false;
            w16.Fill = false;
            // Container child boxCondition.Gtk.Box+BoxChild
            this.entryCondition = new Gtk.Entry();
            this.entryCondition.CanFocus = true;
            this.entryCondition.Name = "entryCondition";
            this.entryCondition.IsEditable = true;
            this.entryCondition.InvisibleChar = '●';
            this.boxCondition.Add(this.entryCondition);
            Gtk.Box.BoxChild w17 = ((Gtk.Box.BoxChild)(this.boxCondition[this.entryCondition]));
            w17.Position = 2;
            this.vbox4.Add(this.boxCondition);
            Gtk.Box.BoxChild w18 = ((Gtk.Box.BoxChild)(this.vbox4[this.boxCondition]));
            w18.Position = 3;
            // Container child vbox4.Gtk.Box+BoxChild
            this.hseparator1 = new Gtk.HSeparator();
            this.hseparator1.Name = "hseparator1";
            this.vbox4.Add(this.hseparator1);
            Gtk.Box.BoxChild w19 = ((Gtk.Box.BoxChild)(this.vbox4[this.hseparator1]));
            w19.Position = 4;
            w19.Expand = false;
            w19.Fill = false;
            // Container child vbox4.Gtk.Box+BoxChild
            this.hbox7 = new Gtk.HBox();
            this.hbox7.Name = "hbox7";
            this.hbox7.Spacing = 6;
            // Container child hbox7.Gtk.Box+BoxChild
            this.label8 = new Gtk.Label();
            this.label8.Name = "label8";
            this.label8.Xalign = 0F;
            this.label8.LabelProp = Mono.Unix.Catalog.GetString("Hit count before breaking:");
            this.hbox7.Add(this.label8);
            Gtk.Box.BoxChild w20 = ((Gtk.Box.BoxChild)(this.hbox7[this.label8]));
            w20.Position = 0;
            w20.Expand = false;
            w20.Fill = false;
            // Container child hbox7.Gtk.Box+BoxChild
            this.spinHitCount = new Gtk.SpinButton(0, 100, 1);
            this.spinHitCount.CanFocus = true;
            this.spinHitCount.Name = "spinHitCount";
            this.spinHitCount.Adjustment.PageIncrement = 10;
            this.spinHitCount.ClimbRate = 1;
            this.spinHitCount.Numeric = true;
            this.spinHitCount.Value = 1;
            this.hbox7.Add(this.spinHitCount);
            Gtk.Box.BoxChild w21 = ((Gtk.Box.BoxChild)(this.hbox7[this.spinHitCount]));
            w21.Position = 1;
            w21.Expand = false;
            w21.Fill = false;
            this.vbox4.Add(this.hbox7);
            Gtk.Box.BoxChild w22 = ((Gtk.Box.BoxChild)(this.vbox4[this.hbox7]));
            w22.Position = 5;
            w22.Expand = false;
            w22.Fill = false;
            this.hbox4.Add(this.vbox4);
            Gtk.Box.BoxChild w23 = ((Gtk.Box.BoxChild)(this.hbox4[this.vbox4]));
            w23.Position = 1;
            this.vbox2.Add(this.hbox4);
            Gtk.Box.BoxChild w24 = ((Gtk.Box.BoxChild)(this.vbox2[this.hbox4]));
            w24.Position = 3;
            w24.Expand = false;
            w24.Fill = false;
            // Container child vbox2.Gtk.Box+BoxChild
            this.label3 = new Gtk.Label();
            this.label3.Name = "label3";
            this.label3.Xalign = 0F;
            this.label3.LabelProp = Mono.Unix.Catalog.GetString("<b>Action</b>");
            this.label3.UseMarkup = true;
            this.vbox2.Add(this.label3);
            Gtk.Box.BoxChild w25 = ((Gtk.Box.BoxChild)(this.vbox2[this.label3]));
            w25.Position = 4;
            w25.Expand = false;
            w25.Fill = false;
            // Container child vbox2.Gtk.Box+BoxChild
            this.hbox2 = new Gtk.HBox();
            this.hbox2.Name = "hbox2";
            this.hbox2.Spacing = 6;
            // Container child hbox2.Gtk.Box+BoxChild
            this.label10 = new Gtk.Label();
            this.label10.WidthRequest = 12;
            this.label10.Name = "label10";
            this.hbox2.Add(this.label10);
            Gtk.Box.BoxChild w26 = ((Gtk.Box.BoxChild)(this.hbox2[this.label10]));
            w26.Position = 0;
            w26.Expand = false;
            w26.Fill = false;
            // Container child hbox2.Gtk.Box+BoxChild
            this.vbox3 = new Gtk.VBox();
            this.vbox3.Name = "vbox3";
            this.vbox3.Spacing = 6;
            // Container child vbox3.Gtk.Box+BoxChild
            this.label11 = new Gtk.Label();
            this.label11.Name = "label11";
            this.label11.Xalign = 0F;
            this.label11.LabelProp = Mono.Unix.Catalog.GetString("When the breakpoint is hit:");
            this.vbox3.Add(this.label11);
            Gtk.Box.BoxChild w27 = ((Gtk.Box.BoxChild)(this.vbox3[this.label11]));
            w27.Position = 0;
            w27.Expand = false;
            w27.Fill = false;
            // Container child vbox3.Gtk.Box+BoxChild
            this.radioActionBreak = new Gtk.RadioButton(Mono.Unix.Catalog.GetString("Break"));
            this.radioActionBreak.CanFocus = true;
            this.radioActionBreak.Name = "radioActionBreak";
            this.radioActionBreak.Active = true;
            this.radioActionBreak.DrawIndicator = true;
            this.radioActionBreak.UseUnderline = true;
            this.radioActionBreak.Group = new GLib.SList(System.IntPtr.Zero);
            this.vbox3.Add(this.radioActionBreak);
            Gtk.Box.BoxChild w28 = ((Gtk.Box.BoxChild)(this.vbox3[this.radioActionBreak]));
            w28.Position = 1;
            w28.Expand = false;
            w28.Fill = false;
            // Container child vbox3.Gtk.Box+BoxChild
            this.radioActionTrace = new Gtk.RadioButton(Mono.Unix.Catalog.GetString("Write expression to the output"));
            this.radioActionTrace.CanFocus = true;
            this.radioActionTrace.Name = "radioActionTrace";
            this.radioActionTrace.DrawIndicator = true;
            this.radioActionTrace.UseUnderline = true;
            this.radioActionTrace.Group = this.radioActionBreak.Group;
            this.vbox3.Add(this.radioActionTrace);
            Gtk.Box.BoxChild w29 = ((Gtk.Box.BoxChild)(this.vbox3[this.radioActionTrace]));
            w29.Position = 2;
            w29.Expand = false;
            w29.Fill = false;
            // Container child vbox3.Gtk.Box+BoxChild
            this.boxTraceExpression = new Gtk.HBox();
            this.boxTraceExpression.Name = "boxTraceExpression";
            this.boxTraceExpression.Spacing = 6;
            // Container child boxTraceExpression.Gtk.Box+BoxChild
            this.label13 = new Gtk.Label();
            this.label13.WidthRequest = 12;
            this.label13.Name = "label13";
            this.boxTraceExpression.Add(this.label13);
            Gtk.Box.BoxChild w30 = ((Gtk.Box.BoxChild)(this.boxTraceExpression[this.label13]));
            w30.Position = 0;
            w30.Expand = false;
            w30.Fill = false;
            // Container child boxTraceExpression.Gtk.Box+BoxChild
            this.label12 = new Gtk.Label();
            this.label12.Name = "label12";
            this.label12.LabelProp = Mono.Unix.Catalog.GetString("Expression:");
            this.boxTraceExpression.Add(this.label12);
            Gtk.Box.BoxChild w31 = ((Gtk.Box.BoxChild)(this.boxTraceExpression[this.label12]));
            w31.Position = 1;
            w31.Expand = false;
            w31.Fill = false;
            // Container child boxTraceExpression.Gtk.Box+BoxChild
            this.entryTraceExpr = new Gtk.Entry();
            this.entryTraceExpr.CanFocus = true;
            this.entryTraceExpr.Name = "entryTraceExpr";
            this.entryTraceExpr.IsEditable = true;
            this.entryTraceExpr.InvisibleChar = '●';
            this.boxTraceExpression.Add(this.entryTraceExpr);
            Gtk.Box.BoxChild w32 = ((Gtk.Box.BoxChild)(this.boxTraceExpression[this.entryTraceExpr]));
            w32.Position = 2;
            this.vbox3.Add(this.boxTraceExpression);
            Gtk.Box.BoxChild w33 = ((Gtk.Box.BoxChild)(this.vbox3[this.boxTraceExpression]));
            w33.Position = 3;
            w33.Expand = false;
            w33.Fill = false;
            this.hbox2.Add(this.vbox3);
            Gtk.Box.BoxChild w34 = ((Gtk.Box.BoxChild)(this.hbox2[this.vbox3]));
            w34.Position = 1;
            this.vbox2.Add(this.hbox2);
            Gtk.Box.BoxChild w35 = ((Gtk.Box.BoxChild)(this.vbox2[this.hbox2]));
            w35.Position = 5;
            w35.Expand = false;
            w35.Fill = false;
            w1.Add(this.vbox2);
            Gtk.Box.BoxChild w36 = ((Gtk.Box.BoxChild)(w1[this.vbox2]));
            w36.Position = 0;
            w36.Expand = false;
            w36.Fill = false;
            // Internal child MonoDevelop.Ide.Gui.Dialogs.BreakpointPropertiesDialog.ActionArea
            Gtk.HButtonBox w37 = this.ActionArea;
            w37.Name = "dialog1_ActionArea";
            w37.Spacing = 6;
            w37.BorderWidth = ((uint)(5));
            w37.LayoutStyle = ((Gtk.ButtonBoxStyle)(4));
            // Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
            this.buttonCancel = new Gtk.Button();
            this.buttonCancel.CanDefault = true;
            this.buttonCancel.CanFocus = true;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseStock = true;
            this.buttonCancel.UseUnderline = true;
            this.buttonCancel.Label = "gtk-cancel";
            this.AddActionWidget(this.buttonCancel, -6);
            Gtk.ButtonBox.ButtonBoxChild w38 = ((Gtk.ButtonBox.ButtonBoxChild)(w37[this.buttonCancel]));
            w38.Expand = false;
            w38.Fill = false;
            // Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
            this.buttonOk = new Gtk.Button();
            this.buttonOk.CanDefault = true;
            this.buttonOk.CanFocus = true;
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseStock = true;
            this.buttonOk.UseUnderline = true;
            this.buttonOk.Label = "gtk-ok";
            w37.Add(this.buttonOk);
            Gtk.ButtonBox.ButtonBoxChild w39 = ((Gtk.ButtonBox.ButtonBoxChild)(w37[this.buttonOk]));
            w39.Position = 1;
            w39.Expand = false;
            w39.Fill = false;
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 508;
            this.DefaultHeight = 505;
            this.Show();
            this.radioBreakAlways.Toggled += new System.EventHandler(this.OnRadioBreakAlwaysToggled);
            this.radioBreakTrue.Toggled += new System.EventHandler(this.OnRadioBreakAlwaysToggled);
            this.radioBreakChange.Toggled += new System.EventHandler(this.OnRadioBreakAlwaysToggled);
            this.radioActionBreak.Toggled += new System.EventHandler(this.OnRadioActionBreakToggled);
            this.radioActionTrace.Toggled += new System.EventHandler(this.OnRadioActionBreakToggled);
            this.buttonOk.Clicked += new System.EventHandler(this.OnButtonOkClicked);
        }
    }
}
