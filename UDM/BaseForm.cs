using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Sandag.TechSvcs.RegionalModels
{
    public partial class BaseForm : Form
    {
        delegate void WriteDelegate(string status);
        public Configuration config;
        public KeyValueConfigurationCollection appSettings;
        public ConnectionStringSettingsCollection connectionStrings;

        #region Constants

        // geography constants
        public int NUM_MGRAS;              // number of mgras
        public int NUM_LUZS;               // Number of LUZs
        public int MAX_MGRAS_IN_LUZ;       // Maximum number of MGRAs in any LUZ 

        // Debugger options
        public bool EMP_CALC_DEBUG;   // Write employment calculations results to file
        public bool MF_DEBUG;     // HS MF debug print flag
        public bool PM_DEBUG;
        public bool UPDATE_DEBUG;
        public bool MEMP_DEBUG;
        public bool RC_DEBUG;         // Write regional controls to file
        public bool SF_DEBUG;         // Write SF distribution to file
        public bool ZB_DEBUG;         // Write luzBase to file
        public bool ZH_DEBUG;         // Write luzHistory to file
        public bool ZT_DEBUG;         // Write luzTemp to file

        /* Travel time curve parameters - these get set at runtime. These values are left over from series 9 and will be recalibrated. */
        public int AUTO_MED;         // Median travel time auto
        public int CBD_MED;          // Median travel time cbd
        public int TRAN_MED;         // Median travel time transit
        public double AUTO_STD;      // Standard deviation auto
        public double CBD_STD;       // Standard deviation cbd
        public double TRAN_STD;      // Standard deviation transit
        public double AUTO_NLA;      // Non-linear adjustment auto
        public double CBD_NLA;       // Non-linear adjustment cbd
        public double TRAN_NLA;      // Non-linear adjustment transit

        // empal parameters
        public double LAGGED_EMP;          // exponent "b" in the EMPAL estimation equation */
        public double ALPHA;
        public double BETA;
        public double LAMBDA;      // We're using a hybrid lambda here - this is a weighted average of lambdas by sector from original EMPAL
                                            // specification with sectors.  This is an override of the estimated
                                        //LAMBDA because the EMPAL equation places too heavy an emphasis on lagged emp and not capacity. *
        public double CAP_EMP;     /* The exponent "a" in the EMPAL estimation equation */

        // other constants

        public int CBDLUZ;           // LUZ id for CBD - uses different curve
        public int INCOMESWITCH;     // default income switch for LUZ income
        public int MAX_TRANS;              /* Max number of transactions for each LUZ in emp, hs allocation */
        public int NUM_HH_INCOME;               // Number of income groups
        public int NUM_EMP_LAND;      /* Number of emp land use categories +1 (0 is a dummy) */
        public int NUM_MF_LAND;       /* Number of mf land use categories +1 (0 is a dummy) */
        public int NUM_EMP_SECTORS;              // Number of civilian employment sectors
        public int NUM_SF_LAND;       /* Number of sf land use categories +1 (0 is a dummy) */
        public int NUM_TT_PROB;      /* Number of bins in the travel time curve probability function */

        #endregion Constants

        #region Instance fields

        public bool minSwitch;
        public int bYear;
        public int fYear;
        public int scenarioID;
        public int luzB;
        public Boolean DoAccessWeights = false;
        public int increment;
        public int[] incrementLabels = { 2012, 2020, 2035, 2050 };
        public double minParam;       // Minimum constraint parameter

        public TableNames TN = new TableNames();

        public string outputLabel;            // Date/time stamp label

        public string networkPath;

        public string[] scenLabels = { "ep", "", "", "", "", "", "", "" };

        public Flags flags;
        public Form caller;
        public Master[] z;                // LUZ data structures
        public Master reg;                // Regional total for LUZ agg data    

        #endregion Instance fields

        public BaseForm()
        {
            InitializeComponent();
        }

        /*****************************************************************************/

        /* method writeToStatusBox() */
        /// <summary>
        /// Method to append a string on a new line to the status box.
        /// </summary>

        /* Revision History
         * 
         * STR             Date       By    Description
         * --------------------------------------------------------------------------
         *                 07/28/03   df    Initial coding
         * --------------------------------------------------------------------------
         */
        public void writeToStatusBox(string status)
        {
            /* If we are running this method from primary thread, no marshalling is
             * needed. */
            if (!txtStatus.InvokeRequired)
            {
                // Append to the string (whose last character is already newLine)
                txtStatus.Text += status + Environment.NewLine;
                // Move the caret to the end of the string, and then scroll to it.
                txtStatus.Select(txtStatus.Text.Length, 0);
                txtStatus.ScrollToCaret();
                Refresh();
            }
            // Invoked from another thread.  Show progress asynchronously.
            else
            {
                WriteDelegate write = new WriteDelegate(writeToStatusBox);
                Invoke(write, new object[] { status });
            }
        }     // End method writeToStatusBox()

        private void UDMBaseForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            caller.Enabled = true;
        }
    }
}
