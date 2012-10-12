﻿namespace GlueCsvEditor.Controls
{
    partial class GridView
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnShowComplexProperties = new System.Windows.Forms.Button();
            this.pgrPropertyEditor = new System.Windows.Forms.PropertyGrid();
            this.cmbCelldata = new System.Windows.Forms.ComboBox();
            this.btnPrependRow = new System.Windows.Forms.Button();
            this.btnDeleteRow = new System.Windows.Forms.Button();
            this.btnAddRow = new System.Windows.Forms.Button();
            this.btnFindNext = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.dgrEditor = new System.Windows.Forms.DataGridView();
            this.txtHeaderType = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lstFilteredTypes = new System.Windows.Forms.ListBox();
            this.btnAddColumn = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.chkIsList = new System.Windows.Forms.CheckBox();
            this.chkIsRequired = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtHeaderName = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrEditor)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnShowComplexProperties);
            this.splitContainer1.Panel1.Controls.Add(this.pgrPropertyEditor);
            this.splitContainer1.Panel1.Controls.Add(this.cmbCelldata);
            this.splitContainer1.Panel1.Controls.Add(this.btnPrependRow);
            this.splitContainer1.Panel1.Controls.Add(this.btnDeleteRow);
            this.splitContainer1.Panel1.Controls.Add(this.btnAddRow);
            this.splitContainer1.Panel1.Controls.Add(this.btnFindNext);
            this.splitContainer1.Panel1.Controls.Add(this.txtSearch);
            this.splitContainer1.Panel1.Controls.Add(this.dgrEditor);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtHeaderType);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.lstFilteredTypes);
            this.splitContainer1.Panel2.Controls.Add(this.btnAddColumn);
            this.splitContainer1.Panel2.Controls.Add(this.btnRemove);
            this.splitContainer1.Panel2.Controls.Add(this.chkIsList);
            this.splitContainer1.Panel2.Controls.Add(this.chkIsRequired);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.txtHeaderName);
            this.splitContainer1.Size = new System.Drawing.Size(676, 525);
            this.splitContainer1.SplitterDistance = 429;
            this.splitContainer1.SplitterWidth = 7;
            this.splitContainer1.TabIndex = 2;
            // 
            // btnShowComplexProperties
            // 
            this.btnShowComplexProperties.Location = new System.Drawing.Point(3, 5);
            this.btnShowComplexProperties.Name = "btnShowComplexProperties";
            this.btnShowComplexProperties.Size = new System.Drawing.Size(25, 23);
            this.btnShowComplexProperties.TabIndex = 0;
            this.btnShowComplexProperties.Text = "...";
            this.btnShowComplexProperties.UseVisualStyleBackColor = true;
            this.btnShowComplexProperties.Click += new System.EventHandler(this.btnShowComplexProperties_Click);
            // 
            // pgrPropertyEditor
            // 
            this.pgrPropertyEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgrPropertyEditor.HelpVisible = false;
            this.pgrPropertyEditor.Location = new System.Drawing.Point(3, 32);
            this.pgrPropertyEditor.Name = "pgrPropertyEditor";
            this.pgrPropertyEditor.Size = new System.Drawing.Size(419, 290);
            this.pgrPropertyEditor.TabIndex = 14;
            this.pgrPropertyEditor.ToolbarVisible = false;
            this.pgrPropertyEditor.Visible = false;
            this.pgrPropertyEditor.Leave += new System.EventHandler(this.pgrPropertyEditor_Leave);
            // 
            // cmbCelldata
            // 
            this.cmbCelldata.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbCelldata.FormattingEnabled = true;
            this.cmbCelldata.Location = new System.Drawing.Point(34, 5);
            this.cmbCelldata.Name = "cmbCelldata";
            this.cmbCelldata.Size = new System.Drawing.Size(124, 21);
            this.cmbCelldata.TabIndex = 1;
            this.cmbCelldata.DropDown += new System.EventHandler(this.cmbCelldata_DropDown);
            this.cmbCelldata.TextChanged += new System.EventHandler(this.cmbCelldata_TextChanged);
            // 
            // btnPrependRow
            // 
            this.btnPrependRow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrependRow.Location = new System.Drawing.Point(3, 495);
            this.btnPrependRow.Name = "btnPrependRow";
            this.btnPrependRow.Size = new System.Drawing.Size(88, 23);
            this.btnPrependRow.TabIndex = 5;
            this.btnPrependRow.Text = "Prepend Row";
            this.btnPrependRow.UseVisualStyleBackColor = true;
            this.btnPrependRow.Click += new System.EventHandler(this.btnPrependRow_Click);
            // 
            // btnDeleteRow
            // 
            this.btnDeleteRow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteRow.Location = new System.Drawing.Point(347, 495);
            this.btnDeleteRow.Name = "btnDeleteRow";
            this.btnDeleteRow.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteRow.TabIndex = 7;
            this.btnDeleteRow.Text = "Delete Row";
            this.btnDeleteRow.UseVisualStyleBackColor = true;
            this.btnDeleteRow.Click += new System.EventHandler(this.btnDeleteRow_Click);
            // 
            // btnAddRow
            // 
            this.btnAddRow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddRow.Location = new System.Drawing.Point(97, 495);
            this.btnAddRow.Name = "btnAddRow";
            this.btnAddRow.Size = new System.Drawing.Size(88, 23);
            this.btnAddRow.TabIndex = 6;
            this.btnAddRow.Text = "Append Row";
            this.btnAddRow.UseVisualStyleBackColor = true;
            this.btnAddRow.Click += new System.EventHandler(this.btnAddRow_Click);
            // 
            // btnFindNext
            // 
            this.btnFindNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFindNext.Location = new System.Drawing.Point(332, 4);
            this.btnFindNext.Name = "btnFindNext";
            this.btnFindNext.Size = new System.Drawing.Size(90, 23);
            this.btnFindNext.TabIndex = 3;
            this.btnFindNext.Text = "Find Next (F3)";
            this.btnFindNext.UseVisualStyleBackColor = true;
            this.btnFindNext.Click += new System.EventHandler(this.btnFindNext_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Location = new System.Drawing.Point(164, 6);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(170, 20);
            this.txtSearch.TabIndex = 2;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            // 
            // dgrEditor
            // 
            this.dgrEditor.AllowUserToAddRows = false;
            this.dgrEditor.AllowUserToDeleteRows = false;
            this.dgrEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgrEditor.Location = new System.Drawing.Point(3, 33);
            this.dgrEditor.Name = "dgrEditor";
            this.dgrEditor.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgrEditor.Size = new System.Drawing.Size(419, 456);
            this.dgrEditor.TabIndex = 4;
            this.dgrEditor.VirtualMode = true;
            this.dgrEditor.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgrEditor_CellBeginEdit);
            this.dgrEditor.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrEditor_CellEnter);
            this.dgrEditor.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgrEditor_CellValueNeeded);
            this.dgrEditor.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgrEditor_CellValuePushed);
            this.dgrEditor.ColumnAdded += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dgrEditor_ColumnAdded);
            this.dgrEditor.ColumnRemoved += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dgrEditor_ColumnRemoved);
            this.dgrEditor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgrEditor_KeyDown);
            this.dgrEditor.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dgrEditor_KeyUp);
            this.dgrEditor.Leave += new System.EventHandler(this.dgrEditor_Leave);
            // 
            // txtHeaderType
            // 
            this.txtHeaderType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHeaderType.Location = new System.Drawing.Point(12, 133);
            this.txtHeaderType.Name = "txtHeaderType";
            this.txtHeaderType.Size = new System.Drawing.Size(191, 20);
            this.txtHeaderType.TabIndex = 11;
            this.txtHeaderType.TextChanged += new System.EventHandler(this.txtHeaderType_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 160);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(167, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "(double click to select type below)";
            // 
            // lstFilteredTypes
            // 
            this.lstFilteredTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstFilteredTypes.FormattingEnabled = true;
            this.lstFilteredTypes.Location = new System.Drawing.Point(12, 176);
            this.lstFilteredTypes.Name = "lstFilteredTypes";
            this.lstFilteredTypes.Size = new System.Drawing.Size(191, 121);
            this.lstFilteredTypes.TabIndex = 12;
            this.lstFilteredTypes.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstFilteredTypes_MouseDoubleClick);
            // 
            // btnAddColumn
            // 
            this.btnAddColumn.Location = new System.Drawing.Point(10, 3);
            this.btnAddColumn.Name = "btnAddColumn";
            this.btnAddColumn.Size = new System.Drawing.Size(75, 23);
            this.btnAddColumn.TabIndex = 8;
            this.btnAddColumn.Text = "Add Column";
            this.btnAddColumn.UseVisualStyleBackColor = true;
            this.btnAddColumn.Click += new System.EventHandler(this.btnAddColumn_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(10, 32);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(102, 23);
            this.btnRemove.TabIndex = 9;
            this.btnRemove.Text = "Remove Column";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // chkIsList
            // 
            this.chkIsList.AutoSize = true;
            this.chkIsList.Location = new System.Drawing.Point(12, 334);
            this.chkIsList.Name = "chkIsList";
            this.chkIsList.Size = new System.Drawing.Size(59, 17);
            this.chkIsList.TabIndex = 14;
            this.chkIsList.Text = "Is List?";
            this.chkIsList.UseVisualStyleBackColor = true;
            this.chkIsList.Click += new System.EventHandler(this.chkIsList_CheckedChanged);
            // 
            // chkIsRequired
            // 
            this.chkIsRequired.AutoSize = true;
            this.chkIsRequired.Location = new System.Drawing.Point(12, 310);
            this.chkIsRequired.Name = "chkIsRequired";
            this.chkIsRequired.Size = new System.Drawing.Size(75, 17);
            this.chkIsRequired.TabIndex = 13;
            this.chkIsRequired.Text = "Required?";
            this.chkIsRequired.UseVisualStyleBackColor = true;
            this.chkIsRequired.Click += new System.EventHandler(this.chkIsRequired_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 116);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Type:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Column Name:";
            // 
            // txtHeaderName
            // 
            this.txtHeaderName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHeaderName.Location = new System.Drawing.Point(12, 77);
            this.txtHeaderName.Name = "txtHeaderName";
            this.txtHeaderName.Size = new System.Drawing.Size(191, 20);
            this.txtHeaderName.TabIndex = 10;
            this.txtHeaderName.TextChanged += new System.EventHandler(this.txtHeaderName_TextChanged);
            // 
            // GridView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "GridView";
            this.Size = new System.Drawing.Size(676, 525);
            this.Load += new System.EventHandler(this.GridView_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrEditor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnShowComplexProperties;
        private System.Windows.Forms.PropertyGrid pgrPropertyEditor;
        private System.Windows.Forms.ComboBox cmbCelldata;
        private System.Windows.Forms.Button btnPrependRow;
        private System.Windows.Forms.Button btnDeleteRow;
        private System.Windows.Forms.Button btnAddRow;
        private System.Windows.Forms.Button btnFindNext;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.DataGridView dgrEditor;
        private System.Windows.Forms.TextBox txtHeaderType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lstFilteredTypes;
        private System.Windows.Forms.Button btnAddColumn;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.CheckBox chkIsList;
        private System.Windows.Forms.CheckBox chkIsRequired;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtHeaderName;
    }
}