namespace VideoDedup
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;
    using global::VideoDedup.TimeSpanExtension;

    public partial class DurationSelection : Form
    {
        private static string ToString(TimeSpan? ts) => ts.Value.ToPrettyString();

        public TimeSpan AbsolutMaximumDuration { get; set; }
        public TimeSpan AbsolutMinimumDuration { get; set; }

        public TimeSpan? SelectedMaximumDuration { get; set; }
        public TimeSpan? SelectedMinimumDuration { get; set; }

        private bool ProgrammaticUpdate { get; set; } = false;

        public DurationSelection() => this.InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
            this.ProgrammaticUpdate = true;

            this.LblDurationInfo.Text = $"Videos are between " +
                $"{ToString(this.AbsolutMinimumDuration)} and " +
                $"{ToString(this.AbsolutMaximumDuration)} long.";

            if (this.SelectedMinimumDuration == null)
            {
                this.SelectedMinimumDuration = this.AbsolutMinimumDuration;
            }
            this.TxtMinimumDuration.Text = ToString(this.SelectedMinimumDuration);

            if (this.SelectedMaximumDuration == null)
            {
                this.SelectedMaximumDuration = this.AbsolutMaximumDuration;
            }
            this.TxtMaximumDuration.Text = ToString(this.SelectedMaximumDuration);

            this.MinimumSlider.Minimum = (int)this.AbsolutMinimumDuration.TotalSeconds;
            this.MinimumSlider.Maximum = (int)this.AbsolutMaximumDuration.TotalSeconds;
            this.MinimumSlider.Value = (int)this.SelectedMinimumDuration.Value.TotalSeconds;

            this.MaximumSlider.Minimum = (int)this.AbsolutMinimumDuration.TotalSeconds;
            this.MaximumSlider.Maximum = (int)this.AbsolutMaximumDuration.TotalSeconds;
            this.MaximumSlider.Value = (int)this.SelectedMaximumDuration.Value.TotalSeconds;

            this.ProgrammaticUpdate = false;

            base.OnLoad(e);
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (!TimeSpan.TryParse(this.TxtMinimumDuration.Text, out var duration))
            {
                _ = MessageBox.Show("Minimum duration is not valid.");
                return;
            }
            this.SelectedMinimumDuration = duration;

            if (!TimeSpan.TryParse(this.TxtMaximumDuration.Text, out duration))
            {
                _ = MessageBox.Show("Maximum duration is not valid.");
                return;
            }
            this.SelectedMaximumDuration = duration;

            if (this.SelectedMaximumDuration <= this.SelectedMinimumDuration)
            {
                var temp = this.SelectedMaximumDuration;
                this.SelectedMaximumDuration = this.SelectedMinimumDuration;
                this.SelectedMinimumDuration = temp;
            }

            this.DialogResult = DialogResult.OK;
        }

        private void MinimumSlider_Scroll(object sender, EventArgs e)
        {
            if (this.ProgrammaticUpdate)
            {
                return;
            }
            this.ProgrammaticUpdate = true;

            this.SelectedMinimumDuration = new TimeSpan(0, 0, this.MinimumSlider.Value);
            this.TxtMinimumDuration.Text = ToString(this.SelectedMinimumDuration);

            this.ProgrammaticUpdate = false;
        }

        private void MaximumSlider_Scroll(object sender, EventArgs e)
        {
            if (this.ProgrammaticUpdate)
            {
                return;
            }
            this.ProgrammaticUpdate = true;

            this.SelectedMaximumDuration = new TimeSpan(0, 0, this.MaximumSlider.Value);
            this.TxtMaximumDuration.Text = ToString(this.SelectedMaximumDuration);

            this.ProgrammaticUpdate = false;
        }

        private void TxtMinimumDuration_TextChanged(object sender, EventArgs e)
        {
            if (this.ProgrammaticUpdate)
            {
                return;
            }

            try
            {
                this.ProgrammaticUpdate = true;

                if (!TimeSpan.TryParse(this.TxtMinimumDuration.Text, out var duration))
                {
                    Debug.Print("Can't parse");
                    return;
                }
                var secs = (int)duration.TotalSeconds;
                if (secs > this.MinimumSlider.Maximum || secs < this.MinimumSlider.Minimum)
                {
                    Debug.Print("Too large or too small");
                    return;
                }
                this.MinimumSlider.Value = secs;

            }
            finally
            {
                this.ProgrammaticUpdate = false;
            }
        }

        private void TxtMaximumDuration_TextChanged(object sender, EventArgs e)
        {
            if (this.ProgrammaticUpdate)
            {
                return;
            }

            try
            {
                this.ProgrammaticUpdate = true;

                if (!TimeSpan.TryParse(this.TxtMaximumDuration.Text, out var duration))
                {
                    return;
                }
                var secs = (int)duration.TotalSeconds;
                if (secs > this.MaximumSlider.Maximum || secs < this.MaximumSlider.Minimum)
                {
                    return;
                }
                this.MaximumSlider.Value = secs;
            }
            finally
            {
                this.ProgrammaticUpdate = false;
            }
        }
    }
}
