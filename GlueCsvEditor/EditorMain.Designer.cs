namespace GlueCsvEditor.Controls
{
    partial class EditorMain
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgrEditor = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.txtHeaderName = new System.Windows.Forms.TextBox();
            this.txtHeaderType = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.chkIsRequired = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgrEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgrEditor
            // 
            this.dgrEditor.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgrEditor.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgrEditor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgrEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgrEditor.Location = new System.Drawing.Point(0, 0);
            this.dgrEditor.Name = "dgrEditor";
            this.dgrEditor.Size = new System.Drawing.Size(357, 493);
            this.dgrEditor.TabIndex = 0;
            this.dgrEditor.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrEditor_CellEndEdit);
            this.dgrEditor.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrEditor_CellEnter);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgrEditor);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.chkIsRequired);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.txtHeaderType);
            this.splitContainer1.Panel2.Controls.Add(this.txtHeaderName);
            this.splitContainer1.Size = new System.Drawing.Size(581, 493);
            this.splitContainer1.SplitterDistance = 357;
            this.splitContainer1.TabIndex = 1;
            // 
            // txtHeaderName
            // 
            this.txtHeaderName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHeaderName.Location = new System.Drawing.Point(85, 29);
            this.txtHeaderName.Name = "txtHeaderName";
            this.txtHeaderName.Size = new System.Drawing.Size(124, 20);
            this.txtHeaderName.TabIndex = 0;
            this.txtHeaderName.TextChanged += new System.EventHandler(this.txtHeaderName_TextChanged);
            // 
            // txtHeaderType
            // 
            this.txtHeaderType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHeaderType.Location = new System.Drawing.Point(85, 55);
            this.txtHeaderType.Name = "txtHeaderType";
            this.txtHeaderType.Size = new System.Drawing.Size(124, 20);
            this.txtHeaderType.TabIndex = 1;
            this.txtHeaderType.TextChanged += new System.EventHandler(this.txtHeaderType_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Column Name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(45, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Type:";
            // 
            // chkIsRequired
            // 
            this.chkIsRequired.AutoSize = true;
            this.chkIsRequired.Location = new System.Drawing.Point(85, 82);
            this.chkIsRequired.Name = "chkIsRequired";
            this.chkIsRequired.Size = new System.Drawing.Size(75, 17);
            this.chkIsRequired.TabIndex = 5;
            this.chkIsRequired.Text = "Required?";
            this.chkIsRequired.UseVisualStyleBackColor = true;
            this.chkIsRequired.CheckedChanged += new System.EventHandler(this.chkIsRequired_CheckedChanged);
            // 
            // EditorMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.splitContainer1);
            this.Name = "EditorMain";
            this.Size = new System.Drawing.Size(581, 493);
            this.Load += new System.EventHandler(this.EditorMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgrEditor)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgrEditor;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtHeaderType;
        private System.Windows.Forms.TextBox txtHeaderName;
        private System.Windows.Forms.CheckBox chkIsRequired;
    }
}
