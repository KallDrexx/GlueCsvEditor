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
            this.tbcViews = new System.Windows.Forms.TabControl();
            this.tabGridView = new System.Windows.Forms.TabPage();
            this.tabEntityView = new System.Windows.Forms.TabPage();
            this.tbcViews.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbcViews
            // 
            this.tbcViews.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tbcViews.Controls.Add(this.tabGridView);
            this.tbcViews.Controls.Add(this.tabEntityView);
            this.tbcViews.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbcViews.Location = new System.Drawing.Point(0, 0);
            this.tbcViews.Name = "tbcViews";
            this.tbcViews.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbcViews.SelectedIndex = 0;
            this.tbcViews.Size = new System.Drawing.Size(660, 493);
            this.tbcViews.TabIndex = 0;
            this.tbcViews.SelectedIndexChanged += new System.EventHandler(this.tbcViews_SelectedIndexChanged);
            // 
            // tabGridView
            // 
            this.tabGridView.Location = new System.Drawing.Point(4, 25);
            this.tabGridView.Name = "tabGridView";
            this.tabGridView.Padding = new System.Windows.Forms.Padding(3);
            this.tabGridView.Size = new System.Drawing.Size(652, 464);
            this.tabGridView.TabIndex = 0;
            this.tabGridView.Text = "Grid View";
            this.tabGridView.UseVisualStyleBackColor = true;
            // 
            // tabEntityView
            // 
            this.tabEntityView.Location = new System.Drawing.Point(4, 25);
            this.tabEntityView.Name = "tabEntityView";
            this.tabEntityView.Padding = new System.Windows.Forms.Padding(3);
            this.tabEntityView.Size = new System.Drawing.Size(652, 464);
            this.tabEntityView.TabIndex = 1;
            this.tabEntityView.Text = "Entity View";
            this.tabEntityView.UseVisualStyleBackColor = true;
            // 
            // EditorMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tbcViews);
            this.Name = "EditorMain";
            this.Size = new System.Drawing.Size(660, 493);
            this.Load += new System.EventHandler(this.EditorMain_Load);
            this.tbcViews.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tbcViews;
        private System.Windows.Forms.TabPage tabGridView;
        private System.Windows.Forms.TabPage tabEntityView;

    }
}
