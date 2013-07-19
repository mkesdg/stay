CREATE TABLE [xref_mgra](
	[MGRA] [int] NOT NULL,
	[City] [tinyint] NULL,
	[ct10] [int] NULL,
	[cityct10] [int] NOT NULL,
	[supersplit] [int] NOT NULL,
	[SG] [int] NULL,
	[ZIP] [int] NULL,
	[Sphere] [smallint] NULL,
	[CPA] [smallint] NULL,
	[CPASG] [int] NULL,
	[Council] [smallint] NULL,
	[Super] [smallint] NULL,
	[LUZ] [smallint] NULL,
	[Elem] [smallint] NULL,
	[Unif] [smallint] NULL,
	[High] [smallint] NULL,
	[Coll] [smallint] NULL,
	[Transit] [smallint] NULL,
	[ct10bg] [int] NULL,
	[cityct10bg] [int] NOT NULL,
	[sra] [tinyint] NULL,
	[msa] [tinyint] NULL,
	[taz] [int] NULL,
	[x] [int] NULL,
	[y] [int] NULL,
 CONSTRAINT [PK_xref_mgra] PRIMARY KEY CLUSTERED 
(
	[MGRA] ASC
))
GO

CREATE TABLE [updateGQC](
	[lckey] [int] NOT NULL,
	[GQC] [int] NULL
)
GO

CREATE TABLE [median_home_prices](
	[luz] [smallint] NOT NULL,
	[ratio] [float] NULL,
 CONSTRAINT [PK_median_home_prices] PRIMARY KEY CLUSTERED 
(
	[luz] ASC
))
GO

CREATE TABLE [sf_inc](
	[luz] [tinyint] NOT NULL,
	[lckey] [int] NOT NULL,
	[dev_code] [tinyint] NULL,
	[lu] [smallint] NULL,
	[pcap] [float] NULL,
	[hs_sf] [int] NULL,
	[hs_mf] [int] NULL,
	[hs_mh] [int] NULL,
	[cap_hs_sf] [int] NULL,
	[cap_hs_mf] [int] NULL,
	[cap_hs_mh] [int] NULL,
	[chg_hs_sf] [int] NULL,
	[chg_hs_mf] [int] NULL,
	[chg_hs_mh] [int] NULL,
 CONSTRAINT [PK_sf_inc] PRIMARY KEY CLUSTERED 
(
	[lckey] ASC
))
GO

CREATE TABLE [sf_dec](
	[luz] [tinyint] NULL,
	[lckey] [int] NOT NULL,
	[dev_code] [tinyint] NULL,
	[hs_sf] [int] NULL,
	[cap_hs_sf] [int] NULL,
	[chg_hs_sf] [int] NULL,
	[pcap_hs] [float] NULL,
 CONSTRAINT [PK_sf_dec] PRIMARY KEY CLUSTERED 
(
	[lckey] ASC
))
GO

CREATE TABLE [reg_fcst](
	[scenario] [tinyint] NOT NULL,
	[year] [smallint] NOT NULL,
	[civ] [int] NULL,
	[mil] [int] NULL,
	[min] [int] NULL,
	[cons] [int] NULL,
	[mfg] [int] NULL,
	[whtrade] [int] NULL,
	[retrade] [int] NULL,
	[twu] [int] NULL,
	[info] [int] NULL,
	[fre] [int] NULL,
	[pbs] [int] NULL,
	[edhs_oth] [int] NULL,
	[edhs_health] [int] NULL,
	[lh_amuse] [int] NULL,
	[lh_hotel] [int] NULL,
	[lh_restaur] [int] NULL,
	[osv_oth] [int] NULL,
	[osv_rel] [int] NULL,
	[gov_fed] [int] NULL,
	[gov_sloth] [int] NULL,
	[gov_sled] [int] NULL,
	[sedw] [int] NULL,
	[hs] [int] NULL,
	[hs_sf] [int] NULL,
	[hs_mf] [int] NULL,
	[hs_mh] [int] NULL,
	[hh] [int] NULL,
	[hh_sf] [int] NULL,
	[hh_mf] [int] NULL,
	[hh_mh] [int] NULL,
	[pop] [int] NULL,
	[hhp] [int] NULL,
	[gq] [int] NULL,
	[gq_civ] [int] NULL,
	[gq_civ_college] [int] NULL,
	[gq_civ_other] [int] NULL,
	[gq_mil] [int] NULL,
	[er] [int] NULL,
	[inc_mean] [int] NULL,
	[inc_median] [int] NULL,
	[i1] [int] NULL,
	[i2] [int] NULL,
	[i3] [int] NULL,
	[i4] [int] NULL,
	[i5] [int] NULL,
	[i6] [int] NULL,
	[i7] [int] NULL,
	[i8] [int] NULL,
	[i9] [int] NULL,
	[i10] [int] NULL,
	[hhs1] [int] NULL,
	[hhs2] [int] NULL,
	[hhs3] [int] NULL,
	[hhs4] [int] NULL,
	[hhs5] [int] NULL,
	[hhs6] [int] NULL,
	[hhs7] [int] NULL,
	[hhwoc] [int] NULL,
	[hhwc] [int] NULL,
	[hhworkers0] [int] NULL,
	[hhworkers1] [int] NULL,
	[hhworkers2] [int] NULL,
	[hhworkers3] [int] NULL,
	[enroll_K8] [int] NULL,
	[enroll_HS] [int] NULL,
	[enroll_MajCol] [int] NULL,
	[enroll_OtherCol] [int] NULL,
	[enroll_AdultEd] [int] NULL,
 CONSTRAINT [PK_reg_fcst_ep] PRIMARY KEY CLUSTERED 
(
	[scenario] ASC,
	[year] ASC
))

CREATE TABLE [prison_temp](
	[mgra] [int] NOT NULL,
	[prison_pop] [int] NULL,
 CONSTRAINT [PK_prison_temp] PRIMARY KEY CLUSTERED 
(
	[mgra] ASC
))
GO

CREATE TABLE [mh_inc](
	[luz] [tinyint] NULL,
	[lckey] [int] NOT NULL,
	[hs_mh] [int] NULL,
	[chg_hs_mh] [int] NULL,
 CONSTRAINT [PK_mh_inc] PRIMARY KEY CLUSTERED 
(
	[lckey] ASC
))
GO

CREATE TABLE [mh_dec](
	[luz] [tinyint] NULL,
	[lckey] [int] NOT NULL,
	[dev_code] [tinyint] NULL,
	[hs_mh] [int] NULL,
	[cap_hs_mh] [int] NULL,
	[chg_hs_mh] [int] NULL,
	[pcap_hs] [float] NULL,
 CONSTRAINT [PK_mh_dec] PRIMARY KEY CLUSTERED 
(
	[lckey] ASC
))
GO

CREATE TABLE [mgrabase_adjusted_for_prisons](
	[scenario] [tinyint] NOT NULL,
	[increment] [int] NOT NULL,
	[mgra] [int] NOT NULL,
	[luz] [smallint] NULL,
	[pop] [int] NULL,
	[hhp] [int] NULL,
	[er] [int] NULL,
	[gq] [int] NULL,
	[gq_civ] [int] NULL,
	[gq_civ_college] [int] NULL,
	[gq_civ_other] [int] NULL,
	[gq_mil] [int] NULL,
	[hs] [int] NULL,
	[hs_sf] [int] NULL,
	[hs_mf] [int] NULL,
	[hs_mh] [int] NULL,
	[hh] [int] NULL,
	[hh_sf] [int] NULL,
	[hh_mf] [int] NULL,
	[hh_mh] [int] NULL,
	[emp] [int] NULL,
	[emp_civ] [int] NULL,
	[emp_mil] [int] NULL,
	[emp_agmin] [int] NULL,
	[emp_cons] [int] NULL,
	[emp_mfg] [int] NULL,
	[emp_whtrade] [int] NULL,
	[emp_retrade] [int] NULL,
	[emp_twu] [int] NULL,
	[emp_info] [int] NULL,
	[emp_fre] [int] NULL,
	[emp_pbs] [int] NULL,
	[emp_edhs_oth] [int] NULL,
	[emp_edhs_health] [int] NULL,
	[emp_lh_amuse] [int] NULL,
	[emp_lh_hotel] [int] NULL,
	[emp_lh_restaur] [int] NULL,
	[emp_osv_oth] [int] NULL,
	[emp_osv_rel] [int] NULL,
	[emp_gov_fed] [int] NULL,
	[emp_gov_sloth] [int] NULL,
	[emp_gov_sled] [int] NULL,
	[emp_sedw] [int] NULL,
	[emp_indus_lu] [int] NULL,
	[emp_comm_lu] [int] NULL,
	[emp_office_lu] [int] NULL,
	[emp_other_lu] [int] NULL,
	[i1] [int] NULL,
	[i2] [int] NULL,
	[i3] [int] NULL,
	[i4] [int] NULL,
	[i5] [int] NULL,
	[i6] [int] NULL,
	[i7] [int] NULL,
	[i8] [int] NULL,
	[i9] [int] NULL,
	[i10] [int] NULL,
	[dev_ldsf] [float] NULL,
	[dev_sf] [float] NULL,
	[dev_mf] [float] NULL,
	[dev_mh] [float] NULL,
	[dev_oth] [float] NULL,
	[dev_ag] [float] NULL,
	[dev_indus] [float] NULL,
	[dev_comm] [float] NULL,
	[dev_office] [float] NULL,
	[dev_schools] [float] NULL,
	[dev_roads] [float] NULL,
	[dev_parks] [float] NULL,
	[dev_mil] [float] NULL,
	[dev_water] [float] NULL,
	[dev_mixed_use] [float] NULL,
	[vac_ldsf] [float] NULL,
	[vac_sf] [float] NULL,
	[vac_mf] [float] NULL,
	[vac_mh] [float] NULL,
	[vac_oth] [float] NULL,
	[vac_ag] [float] NULL,
	[vac_indus] [float] NULL,
	[vac_comm] [float] NULL,
	[vac_office] [float] NULL,
	[vac_schools] [float] NULL,
	[vac_roads] [float] NULL,
	[vac_mixed_use] [float] NULL,
	[vac_parks] [float] NULL,
	[redev_sf_mf] [float] NULL,
	[redev_sf_emp] [float] NULL,
	[redev_mf_emp] [float] NULL,
	[redev_mh_sf] [float] NULL,
	[redev_mh_mf] [float] NULL,
	[redev_mh_emp] [float] NULL,
	[redev_ag_ldsf] [float] NULL,
	[redev_ag_sf] [float] NULL,
	[redev_ag_mf] [float] NULL,
	[redev_ag_indus] [float] NULL,
	[redev_ag_comm] [float] NULL,
	[redev_ag_office] [float] NULL,
	[redev_ag_schools] [float] NULL,
	[redev_ag_roads] [float] NULL,
	[redev_emp_res] [float] NULL,
	[redev_emp_emp] [float] NULL,
	[infill_sf] [float] NULL,
	[infill_mf] [float] NULL,
	[infill_emp] [float] NULL,
	[acres] [float] NULL,
	[dev] [float] NULL,
	[vac] [float] NULL,
	[unusable] [float] NULL,
 CONSTRAINT [PK_mgrabase_adusted_for_prisons] PRIMARY KEY CLUSTERED 
(
	[scenario] ASC,
	[increment] ASC,
	[mgra] ASC
))

CREATE TABLE [mf_inc](
	[luz] [tinyint] NULL,
	[lckey] [int] NOT NULL,
	[dev_code] [tinyint] NULL,
	[lu] [smallint] NULL,
	[pcap] [float] NULL,
	[hs_sf] [int] NULL,
	[hs_mf] [int] NULL,
	[hs_mh] [int] NULL,
	[cap_hs_sf] [int] NULL,
	[cap_hs_mf] [int] NULL,
	[cap_hs_mh] [int] NULL,
	[chg_hs_sf] [int] NULL,
	[chg_hs_mf] [int] NULL,
	[chg_hs_mh] [int] NULL,
 CONSTRAINT [PK_mf_inc] PRIMARY KEY CLUSTERED 
(
	[lckey] ASC
))

CREATE TABLE [mf_dec](
	[luz] [tinyint] NULL,
	[lckey] [int] NOT NULL,
	[dev_code] [tinyint] NULL,
	[hs_mf] [int] NULL,
	[cap_hs_mf] [int] NULL,
	[chg_hs_mf] [int] NULL,
	[pcap_hs] [float] NULL,
 CONSTRAINT [PK_mf_dec] PRIMARY KEY CLUSTERED 
(
	[lckey] ASC
))

CREATE TABLE [luzbase](
	[scenario] [tinyint] NOT NULL,
	[increment] [int] NOT NULL,
	[luz] [smallint] NOT NULL,
	[pop] [int] NOT NULL,
	[hhp] [int] NOT NULL,
	[er] [int] NOT NULL,
	[gq] [int] NOT NULL,
	[gq_civ] [int] NOT NULL,
	[gq_mil] [int] NOT NULL,
	[gq_civ_college] [int] NULL,
	[gq_civ_other] [int] NULL,
	[hs] [int] NOT NULL,
	[hs_sf] [int] NOT NULL,
	[hs_mf] [int] NOT NULL,
	[hs_mh] [int] NULL,
	[hh] [int] NOT NULL,
	[hh_sf] [int] NOT NULL,
	[hh_mf] [int] NOT NULL,
	[hh_mh] [int] NOT NULL,
	[inc_median] [int] NOT NULL,
	[asd] [float] NOT NULL,
	[nla] [float] NOT NULL,
	[i1] [int] NOT NULL,
	[i2] [int] NOT NULL,
	[i3] [int] NOT NULL,
	[i4] [int] NOT NULL,
	[i5] [int] NOT NULL,
	[i6] [int] NOT NULL,
	[i7] [int] NOT NULL,
	[i8] [int] NOT NULL,
	[i9] [int] NOT NULL,
	[i10] [int] NOT NULL,
	[emp] [int] NOT NULL,
	[emp_civ] [int] NOT NULL,
	[emp_mil] [int] NOT NULL,
	[emp_agmin] [int] NOT NULL,
	[emp_cons] [int] NOT NULL,
	[emp_mfg] [int] NOT NULL,
	[emp_whtrade] [int] NOT NULL,
	[emp_retrade] [int] NOT NULL,
	[emp_twu] [int] NOT NULL,
	[emp_info] [int] NOT NULL,
	[emp_fre] [int] NOT NULL,
	[emp_pbs] [int] NOT NULL,
	[emp_edhs_oth] [int] NOT NULL,
	[emp_edhs_health] [int] NULL,
	[emp_lh_amuse] [int] NOT NULL,
	[emp_lh_hotel] [int] NULL,
	[emp_lh_restaur] [int] NULL,
	[emp_osv_oth] [int] NOT NULL,
	[emp_osv_rel] [int] NULL,
	[emp_gov_fed] [int] NOT NULL,
	[emp_gov_sloth] [int] NULL,
	[emp_gov_sled] [int] NULL,
	[emp_sedw] [int] NOT NULL,
 CONSTRAINT [PK_luzbase] PRIMARY KEY CLUSTERED 
(
	[scenario] ASC,
	[increment] ASC,
	[luz] ASC
))

CREATE TABLE [luz_temp](
	[scenario] [tinyint] NOT NULL,
	[increment] [int] NOT NULL,
	[type] [int] NOT NULL,
	[id] [smallint] NOT NULL,
	[site_civ] [int] NULL,
	[site_sf] [int] NULL,
	[site_mf] [int] NULL,
	[site_mh] [int] NULL,
	[site_gq_civ] [int] NULL,
	[site_gq_mil] [int] NULL,
	[site_acres_emp_lu1] [float] NULL,
	[site_acres_emp_lu2] [float] NULL,
	[site_acres_emp_lu3] [float] NULL,
	[site_acres_emp_lu4] [float] NULL,
	[site_acres_emp_lu5] [float] NULL,
	[site_acres_emp_lu6] [float] NULL,
	[site_acres_emp_lu7] [float] NULL,
	[site_acres_emp_tot] [float] NULL,
	[site_acres_sf_lu1] [float] NULL,
	[site_acres_sf_lu2] [float] NULL,
	[site_acres_sf_lu3] [float] NULL,
	[site_acres_sf_lu4] [float] NULL,
	[site_acres_sf_tot] [float] NULL,
	[site_acres_mf_lu1] [float] NULL,
	[site_acres_mf_lu2] [float] NULL,
	[site_acres_mf_lu3] [float] NULL,
	[site_acres_mf_tot] [float] NULL,
	[cap_emp_lu1] [int] NULL,
	[cap_emp_lu2] [int] NULL,
	[cap_emp_lu3] [int] NULL,
	[cap_emp_lu4] [int] NULL,
	[cap_emp_lu5] [int] NULL,
	[cap_emp_lu6] [int] NULL,
	[cap_emp_lu7] [int] NULL,
	[cap_emp_tot] [int] NULL,
	[cap_sf_lu1] [int] NULL,
	[cap_sf_lu2] [int] NULL,
	[cap_sf_lu3] [int] NULL,
	[cap_sf_lu4] [int] NULL,
	[cap_sf_tot] [int] NULL,
	[cap_mf_lu1] [int] NULL,
	[cap_mf_lu2] [int] NULL,
	[cap_mf_lu3] [int] NULL,
	[cap_mf_tot] [int] NULL,
	[acres_emp_lu1] [float] NULL,
	[acres_emp_lu2] [float] NULL,
	[acres_emp_lu3] [float] NULL,
	[acres_emp_lu4] [float] NULL,
	[acres_emp_lu5] [float] NULL,
	[acres_emp_lu6] [float] NULL,
	[acres_emp_lu7] [float] NULL,
	[acres_emp_tot] [float] NULL,
	[acres_sf_lu1] [float] NULL,
	[acres_sf_lu2] [float] NULL,
	[acres_sf_lu3] [float] NULL,
	[acres_sf_lu4] [float] NULL,
	[acres_sf_tot] [float] NULL,
	[acres_mf_lu1] [float] NULL,
	[acres_mf_lu2] [float] NULL,
	[acres_mf_lu3] [float] NULL,
	[acres_mf_tot] [float] NULL,
 CONSTRAINT [PK_zum_temp_2008_ep] PRIMARY KEY CLUSTERED 
(
	[scenario] ASC,
	[increment] ASC,
	[type] ASC,
	[id] ASC
))

CREATE TABLE [luz_income_parms](
	[luz] [smallint] NOT NULL,
	[inc_median] [int] NULL,
	[asd] [float] NULL,
	[nla] [float] NULL,
	[i1] [int] NULL,
	[i2] [int] NULL,
	[i3] [int] NULL,
	[i4] [int] NULL,
	[i5] [int] NULL,
	[i6] [int] NULL,
	[i7] [int] NULL,
	[i8] [int] NULL,
	[i9] [int] NULL,
	[i10] [int] NULL,
	[diff1] [float] NULL,
	[diff2] [float] NULL,
	[diff3] [float] NULL,
	[diff4] [float] NULL,
	[diff5] [float] NULL,
	[diff6] [float] NULL,
	[diff7] [float] NULL,
	[diff8] [float] NULL,
	[diff9] [float] NULL,
	[diff10] [float] NULL,
 CONSTRAINT [PK_zum_inc_parms] PRIMARY KEY CLUSTERED 
(
	[luz] ASC
))

CREATE TABLE [luz_hs_sf_ovr](
	[scenario] [tinyint] NOT NULL,
	[increment] [smallint] NOT NULL,
	[luz] [smallint] NOT NULL,
	[sf_ovr] [int] NULL,
 CONSTRAINT [PK_zum_mf_ovr] PRIMARY KEY CLUSTERED 
(
	[scenario] ASC,
	[increment] ASC,
	[luz] ASC
))

CREATE TABLE [luz_hs_mf_ovr](
	[scenario] [tinyint] NOT NULL,
	[increment] [smallint] NOT NULL,
	[luz] [smallint] NOT NULL,
	[mf_ovr] [int] NULL,
 CONSTRAINT [PK_luz_hs_mf_ovr] PRIMARY KEY CLUSTERED 
(
	[scenario] ASC,
	[increment] ASC,
	[luz] ASC
))

CREATE TABLE [luz_hist](
	[scenario] [tinyint] NOT NULL,
	[increment] [int] NOT NULL,
	[luz] [smallint] NOT NULL,
	[civ_y5] [int] NULL,
	[civ_y0] [int] NULL,
	[sf_y5] [int] NULL,
	[sf_y0] [int] NULL,
	[mf_y5] [int] NULL,
	[mf_y0] [int] NULL,
	[hh_y5] [int] NULL,
	[hh_y0] [int] NULL,
 CONSTRAINT [PK_zumhist_2008_ep] PRIMARY KEY CLUSTERED 
(
	[scenario] ASC,
	[increment] ASC,
	[luz] ASC
))

CREATE TABLE [luz_emp_lu_ovr](
	[scenario] [tinyint] NOT NULL,
	[increment] [smallint] NOT NULL,
	[luz] [smallint] NOT NULL,
	[civ] [int] NULL,
	[elu1] [int] NULL,
	[elu2] [int] NULL,
	[elu3] [int] NULL,
	[elu4] [int] NULL,
	[elu5] [int] NULL,
	[elu6] [int] NULL,
 CONSTRAINT [PK_zum_emp_lu_ovr] PRIMARY KEY CLUSTERED 
(
	[scenario] ASC,
	[increment] ASC,
	[luz] ASC
))

CREATE TABLE [luz_dc_vac_ovr](
	[scenario] [tinyint] NOT NULL,
	[increment] [smallint] NOT NULL,
	[luz] [smallint] NOT NULL,
	[vac_sf] [float] NULL,
	[vac_mf] [float] NULL,
	[vac_mh] [float] NULL,
 CONSTRAINT [PK_zum_dc_vac_ovr] PRIMARY KEY CLUSTERED 
(
	[scenario] ASC,
	[increment] ASC,
	[luz] ASC
))

CREATE TABLE [luz_dc_inc_ovr](
	[scenario] [tinyint] NOT NULL,
	[increment] [smallint] NOT NULL,
	[luz] [smallint] NOT NULL,
	[income_median] [int] NULL,
	[asd] [float] NULL,
	[nla] [float] NULL,
	[income_switch] [tinyint] NULL,
 CONSTRAINT [PK_zum_dc_inc_ovr] PRIMARY KEY CLUSTERED 
(
	[scenario] ASC,
	[increment] ASC,
	[luz] ASC
))

CREATE TABLE [luz_dc_hhs_ovr](
	[scenario] [tinyint] NOT NULL,
	[increment] [smallint] NOT NULL,
	[luz] [smallint] NOT NULL,
	[hhs] [float] NULL,
 CONSTRAINT [PK_zum_dc_hhs_ovr] PRIMARY KEY CLUSTERED 
(
	[scenario] ASC,
	[increment] ASC,
	[luz] ASC
))

CREATE TABLE [luz_dc_er_ovr](
	[scenario] [tinyint] NOT NULL,
	[increment] [smallint] NOT NULL,
	[luz] [smallint] NOT NULL,
	[rate] [float] NULL,
 CONSTRAINT [PK_zum_dc_er_ovr] PRIMARY KEY CLUSTERED 
(
	[scenario] ASC,
	[luz] ASC,
	[increment] ASC
))

CREATE TABLE [luz_dc_emp_sector_ovr](
	[scenario] [tinyint] NOT NULL,
	[increment] [int] NOT NULL,
	[luz] [smallint] NOT NULL,
	[e1] [int] NULL,
	[e2] [int] NULL,
	[e3] [int] NULL,
	[e4] [int] NULL,
	[e5] [int] NULL,
	[e6] [int] NULL,
	[e7] [int] NULL,
	[e8] [int] NULL,
	[e9] [int] NULL,
	[e10] [int] NULL,
	[e11] [int] NULL,
	[e12] [int] NULL,
	[e13] [int] NULL,
	[e14] [int] NULL,
	[e15] [int] NULL,
	[e16] [int] NULL,
	[e17] [int] NULL,
	[e18] [int] NULL,
	[e19] [int] NULL,
	[e20] [int] NULL,
 CONSTRAINT [PK_luz_dc_emp_sector_ovr] PRIMARY KEY CLUSTERED 
(
	[scenario] ASC,
	[increment] ASC,
	[luz] ASC
))

CREATE TABLE [income_distribution_adjustments](
	[i1] [float] NULL,
	[i2] [float] NULL,
	[i3] [float] NULL,
	[i4] [float] NULL,
	[i5] [float] NULL,
	[i6] [float] NULL,
	[i7] [float] NULL,
	[i8] [float] NULL,
	[i9] [float] NULL,
	[i10] [float] NULL
)

CREATE TABLE [imped_tran](
	[scenario] [tinyint] NOT NULL,
	[increment] [int] NOT NULL,
	[orig] [smallint] NOT NULL,
	[dest] [smallint] NOT NULL,
	[time] [smallint] NULL,
 CONSTRAINT [PK_imped_tran] PRIMARY KEY CLUSTERED 
(
	[scenario] ASC,
	[increment] ASC,
	[orig] ASC,
	[dest] ASC
))

CREATE TABLE [imped_pm](
	[scenario] [tinyint] NOT NULL,
	[increment] [int] NOT NULL,
	[orig] [smallint] NOT NULL,
	[dest] [smallint] NOT NULL,
	[time] [smallint] NULL,
 CONSTRAINT [PK_imped_pm] PRIMARY KEY CLUSTERED 
(
	[scenario] ASC,
	[increment] ASC,
	[orig] ASC,
	[dest] ASC
))

CREATE TABLE [imped_am](
	[scenario] [tinyint] NOT NULL,
	[increment] [int] NOT NULL,
	[orig] [smallint] NOT NULL,
	[dest] [smallint] NOT NULL,
	[time] [smallint] NULL,
 CONSTRAINT [PK_imped_am_2008_ep] PRIMARY KEY CLUSTERED 
(
	[scenario] ASC,
	[increment] ASC,
	[orig] ASC,
	[dest] ASC
))

CREATE TABLE [fractee](
	[scenario] [tinyint] NOT NULL,
	[increment] [int] NOT NULL,
	[luz] [smallint] NOT NULL,
	[fraction] [float] NULL,
 CONSTRAINT [PK_fractee_2008_ep] PRIMARY KEY CLUSTERED 
(
	[scenario] ASC,
	[increment] ASC,
	[luz] ASC
))

CREATE TABLE [emp_inc](
	[luz] [tinyint] NULL,
	[lckey] [int] NOT NULL,
	[dev_code] [tinyint] NULL,
	[lu] [smallint] NULL,
	[emp_civ] [int] NULL,
	[cap_emp_civ] [int] NULL,
	[pcap] [float] NULL,
	[chg_emp_civ] [int] NULL,
	[hs_sf] [int] NULL,
	[hs_mf] [int] NULL,
	[hs_mh] [int] NULL,
	[cap_hs_sf] [int] NULL,
	[cap_hs_mf] [int] NULL,
	[cap_hs_mh] [int] NULL,
	[chg_hs_sf] [int] NULL,
	[chg_hs_mf] [int] NULL,
	[chg_hs_mh] [int] NULL,
 CONSTRAINT [PK_emp_inc] PRIMARY KEY CLUSTERED 
(
	[lckey] ASC
))

CREATE TABLE [emp_distribution_by_lu](
	[plu] [smallint] NOT NULL,
	[ag] [float] NULL,
	[con] [float] NULL,
	[mfg] [float] NULL,
	[whtrade] [float] NULL,
	[retrade] [float] NULL,
	[twu] [float] NULL,
	[info] [float] NULL,
	[fre] [float] NULL,
	[pbs] [float] NULL,
	[edhs_other] [float] NULL,
	[edhs_health] [float] NULL,
	[lh_amuse] [float] NULL,
	[lh_hotel] [float] NULL,
	[lh_restaur] [float] NULL,
	[osv_ps] [float] NULL,
	[osv_rs] [float] NULL,
	[gov_fed] [float] NULL,
	[gov_sloth] [float] NULL,
	[gov_sled] [float] NULL,
	[mil] [float] NULL,
	[sedw] [float] NULL,
 CONSTRAINT [PK_emp_lu] PRIMARY KEY CLUSTERED 
(
	[plu] ASC
))

CREATE TABLE [emp_dec](
	[lckey] [int] NOT NULL,
	[dev_code] [tinyint] NULL,
	[emp_civ] [int] NULL,
	[chg_emp_civ] [int] NULL,
	[cap_emp_civ] [int] NULL,
	[pcap] [float] NULL,
 CONSTRAINT [PK_emp_dec] PRIMARY KEY CLUSTERED 
(
	[lckey] ASC
))

CREATE TABLE [capacity_4](
	[scenario] [tinyint] NOT NULL,
	[increment] [int] NOT NULL,
	[LCKey] [int] NOT NULL,
	[planid] [float] NULL,
	[mgra] [smallint] NOT NULL,
	[luz] [smallint] NOT NULL,
	[sphere] [smallint] NOT NULL,
	[site] [smallint] NOT NULL,
	[dev_code] [tinyint] NOT NULL,
	[lu] [smallint] NOT NULL,
	[plu] [smallint] NOT NULL,
	[udm_emp_lu] [tinyint] NOT NULL,
	[udm_sf_lu] [tinyint] NOT NULL,
	[udm_mf_lu] [tinyint] NOT NULL,
	[phase] [smallint] NOT NULL,
	[devyear] [smallint] NOT NULL,
	[loden] [float] NOT NULL,
	[hiden] [float] NOT NULL,
	[empden] [float] NOT NULL,
	[actden] [float] NOT NULL,
	[acres] [float] NOT NULL,
	[parcel_acres] [float] NULL,
	[effective_acres] [float] NULL,
	[percent_constrained] [float] NULL,
	[pcap_hs] [float] NOT NULL,
	[pcap_emp] [float] NULL,
	[emp_civ] [int] NOT NULL,
	[emp_mil] [int] NULL,
	[hs] [int] NOT NULL,
	[hs_sf] [int] NOT NULL,
	[hs_mf] [int] NOT NULL,
	[hs_mh] [int] NOT NULL,
	[gq_civ] [int] NOT NULL,
	[gq_mil] [int] NOT NULL,
	[cap_hs] [int] NOT NULL,
	[cap_hs_sf] [int] NOT NULL,
	[cap_hs_mf] [int] NOT NULL,
	[cap_hs_mh] [int] NOT NULL,
	[cap_emp_civ] [int] NOT NULL,
	[chg_emp_civ] [int] NOT NULL,
	[chg_hs_sf] [int] NOT NULL,
	[chg_hs_mf] [int] NOT NULL,
	[chg_hs_mh] [int] NOT NULL,
	[net_flag] [tinyint] NULL,
	[mktstat] [int] NULL,
	[siteLU] [smallint] NULL,
	[siteSF] [int] NULL,
	[siteMF] [int] NULL,
	[siteMH] [int] NULL,
	[siteGQCiv] [int] NULL,
	[siteGQMil] [int] NULL,
	[siteEmp] [int] NULL,
	[siteMil] [int] NULL,
 CONSTRAINT [PK_capacity13_4] PRIMARY KEY CLUSTERED 
(
	[scenario] ASC,
	[increment] ASC,
	[LCKey] ASC
))

CREATE TABLE [capacity_3](
	[scenario] [tinyint] NOT NULL,
	[increment] [int] NOT NULL,
	[LCKey] [int] NOT NULL,
	[planid] [float] NULL,
	[mgra] [smallint] NOT NULL,
	[luz] [smallint] NOT NULL,
	[sphere] [smallint] NOT NULL,
	[site] [smallint] NOT NULL,
	[dev_code] [tinyint] NOT NULL,
	[lu] [smallint] NOT NULL,
	[plu] [smallint] NOT NULL,
	[udm_emp_lu] [tinyint] NOT NULL,
	[udm_sf_lu] [tinyint] NOT NULL,
	[udm_mf_lu] [tinyint] NOT NULL,
	[phase] [smallint] NOT NULL,
	[devyear] [smallint] NOT NULL,
	[loden] [float] NOT NULL,
	[hiden] [float] NOT NULL,
	[empden] [float] NOT NULL,
	[actden] [float] NOT NULL,
	[acres] [float] NOT NULL,
	[parcel_acres] [float] NULL,
	[effective_acres] [float] NULL,
	[percent_constrained] [float] NULL,
	[pcap_hs] [float] NOT NULL,
	[pcap_emp] [float] NULL,
	[emp_civ] [int] NOT NULL,
	[emp_mil] [int] NULL,
	[hs] [int] NOT NULL,
	[hs_sf] [int] NOT NULL,
	[hs_mf] [int] NOT NULL,
	[hs_mh] [int] NOT NULL,
	[gq_civ] [int] NOT NULL,
	[gq_mil] [int] NOT NULL,
	[cap_hs] [int] NOT NULL,
	[cap_hs_sf] [int] NOT NULL,
	[cap_hs_mf] [int] NOT NULL,
	[cap_hs_mh] [int] NOT NULL,
	[cap_emp_civ] [int] NOT NULL,
	[chg_emp_civ] [int] NOT NULL,
	[chg_hs_sf] [int] NOT NULL,
	[chg_hs_mf] [int] NOT NULL,
	[chg_hs_mh] [int] NOT NULL,
	[net_flag] [tinyint] NULL,
	[mktstat] [int] NULL,
	[siteLU] [smallint] NULL,
	[siteSF] [int] NULL,
	[siteMF] [int] NULL,
	[siteMH] [int] NULL,
	[siteGQCiv] [int] NULL,
	[siteGQMil] [int] NULL,
	[siteEmp] [int] NULL,
	[siteMil] [int] NULL,
 CONSTRAINT [PK_capacity13_3] PRIMARY KEY CLUSTERED 
(
	[scenario] ASC,
	[increment] ASC,
	[LCKey] ASC
))

CREATE TABLE [capacity_2](
	[scenario] [tinyint] NOT NULL,
	[increment] [int] NOT NULL,
	[LCKey] [int] NOT NULL,
	[planid] [float] NULL,
	[mgra] [smallint] NOT NULL,
	[luz] [smallint] NOT NULL,
	[sphere] [smallint] NOT NULL,
	[site] [smallint] NOT NULL,
	[dev_code] [tinyint] NOT NULL,
	[lu] [smallint] NOT NULL,
	[plu] [smallint] NOT NULL,
	[udm_emp_lu] [tinyint] NOT NULL,
	[udm_sf_lu] [tinyint] NOT NULL,
	[udm_mf_lu] [tinyint] NOT NULL,
	[phase] [smallint] NOT NULL,
	[devyear] [smallint] NOT NULL,
	[loden] [float] NOT NULL,
	[hiden] [float] NOT NULL,
	[empden] [float] NOT NULL,
	[actden] [float] NOT NULL,
	[acres] [float] NOT NULL,
	[parcel_acres] [float] NULL,
	[effective_acres] [float] NULL,
	[percent_constrained] [float] NULL,
	[pcap_hs] [float] NOT NULL,
	[pcap_emp] [float] NULL,
	[emp_civ] [int] NOT NULL,
	[emp_mil] [int] NULL,
	[hs] [int] NOT NULL,
	[hs_sf] [int] NOT NULL,
	[hs_mf] [int] NOT NULL,
	[hs_mh] [int] NOT NULL,
	[gq_civ] [int] NOT NULL,
	[gq_mil] [int] NOT NULL,
	[cap_hs] [int] NOT NULL,
	[cap_hs_sf] [int] NOT NULL,
	[cap_hs_mf] [int] NOT NULL,
	[cap_hs_mh] [int] NOT NULL,
	[cap_emp_civ] [int] NOT NULL,
	[chg_emp_civ] [int] NOT NULL,
	[chg_hs_sf] [int] NOT NULL,
	[chg_hs_mf] [int] NOT NULL,
	[chg_hs_mh] [int] NOT NULL,
	[net_flag] [tinyint] NULL,
	[mktstat] [int] NULL,
	[siteLU] [smallint] NULL,
	[siteSF] [int] NULL,
	[siteMF] [int] NULL,
	[siteMH] [int] NULL,
	[siteGQCiv] [int] NULL,
	[siteGQMil] [int] NULL,
	[siteEmp] [int] NULL,
	[siteMil] [int] NULL,
 CONSTRAINT [PK_capacity13_2] PRIMARY KEY CLUSTERED 
(
	[scenario] ASC,
	[increment] ASC,
	[LCKey] ASC
))

CREATE TABLE [capacity_1](
	[scenario] [tinyint] NOT NULL,
	[increment] [int] NOT NULL,
	[LCKey] [int] NOT NULL,
	[planid] [float] NULL,
	[mgra] [smallint] NOT NULL,
	[luz] [smallint] NOT NULL,
	[sphere] [smallint] NOT NULL,
	[site] [smallint] NOT NULL,
	[dev_code] [tinyint] NOT NULL,
	[lu] [smallint] NOT NULL,
	[plu] [smallint] NOT NULL,
	[udm_emp_lu] [tinyint] NOT NULL,
	[udm_sf_lu] [tinyint] NOT NULL,
	[udm_mf_lu] [tinyint] NOT NULL,
	[phase] [smallint] NOT NULL,
	[devyear] [smallint] NOT NULL,
	[loden] [float] NOT NULL,
	[hiden] [float] NOT NULL,
	[empden] [float] NOT NULL,
	[actden] [float] NOT NULL,
	[acres] [float] NOT NULL,
	[parcel_acres] [float] NULL,
	[effective_acres] [float] NULL,
	[percent_constrained] [float] NULL,
	[pcap_hs] [float] NOT NULL,
	[pcap_emp] [float] NULL,
	[emp_civ] [int] NOT NULL,
	[emp_mil] [int] NULL,
	[hs] [int] NOT NULL,
	[hs_sf] [int] NOT NULL,
	[hs_mf] [int] NOT NULL,
	[hs_mh] [int] NOT NULL,
	[gq_civ] [int] NOT NULL,
	[gq_mil] [int] NOT NULL,
	[cap_hs] [int] NOT NULL,
	[cap_hs_sf] [int] NOT NULL,
	[cap_hs_mf] [int] NOT NULL,
	[cap_hs_mh] [int] NOT NULL,
	[cap_emp_civ] [int] NOT NULL,
	[chg_emp_civ] [int] NOT NULL,
	[chg_hs_sf] [int] NOT NULL,
	[chg_hs_mf] [int] NOT NULL,
	[chg_hs_mh] [int] NOT NULL,
	[net_flag] [tinyint] NULL,
	[mktstat] [int] NULL,
	[siteLU] [smallint] NULL,
	[siteSF] [int] NULL,
	[siteMF] [int] NULL,
	[siteMH] [int] NULL,
	[siteGQCiv] [int] NULL,
	[siteGQMil] [int] NULL,
	[siteEmp] [int] NULL,
	[siteMil] [int] NULL,
 CONSTRAINT [PK_capacity13_1] PRIMARY KEY CLUSTERED 
(
	[scenario] ASC,
	[increment] ASC,
	[LCKey] ASC
))

CREATE TABLE [capacity](
	[scenario] [tinyint] NOT NULL,
	[increment] [int] NOT NULL,
	[LCKey] [int] NOT NULL,
	[planid] [float] NULL,
	[mgra] [smallint] NOT NULL,
	[luz] [smallint] NOT NULL,
	[sphere] [smallint] NOT NULL,
	[site] [smallint] NOT NULL,
	[dev_code] [tinyint] NOT NULL,
	[lu] [smallint] NOT NULL,
	[plu] [smallint] NOT NULL,
	[udm_emp_lu] [tinyint] NOT NULL,
	[udm_sf_lu] [tinyint] NOT NULL,
	[udm_mf_lu] [tinyint] NOT NULL,
	[phase] [smallint] NOT NULL,
	[devyear] [smallint] NOT NULL,
	[loden] [float] NOT NULL,
	[hiden] [float] NOT NULL,
	[empden] [float] NOT NULL,
	[actden] [float] NOT NULL,
	[acres] [float] NOT NULL,
	[parcel_acres] [float] NULL,
	[effective_acres] [float] NULL,
	[percent_constrained] [float] NULL,
	[pcap_hs] [float] NOT NULL,
	[pcap_emp] [float] NULL,
	[emp_civ] [int] NOT NULL,
	[emp_mil] [int] NULL,
	[hs] [int] NOT NULL,
	[hs_sf] [int] NOT NULL,
	[hs_mf] [int] NOT NULL,
	[hs_mh] [int] NOT NULL,
	[gq_civ] [int] NOT NULL,
	[gq_mil] [int] NOT NULL,
	[cap_hs] [int] NOT NULL,
	[cap_hs_sf] [int] NOT NULL,
	[cap_hs_mf] [int] NOT NULL,
	[cap_hs_mh] [int] NOT NULL,
	[cap_emp_civ] [int] NOT NULL,
	[chg_emp_civ] [int] NOT NULL,
	[chg_hs_sf] [int] NOT NULL,
	[chg_hs_mf] [int] NOT NULL,
	[chg_hs_mh] [int] NOT NULL,
	[net_flag] [tinyint] NULL,
	[mktstat] [int] NULL,
	[siteLU] [smallint] NULL,
	[siteSF] [int] NULL,
	[siteMF] [int] NULL,
	[siteMH] [int] NULL,
	[siteGQCiv] [int] NULL,
	[siteGQMil] [int] NULL,
	[siteEmp] [int] NULL,
	[siteMil] [int] NULL,
 CONSTRAINT [PK_capacity] PRIMARY KEY CLUSTERED 
(
	[scenario] ASC,
	[increment] ASC,
	[LCKey] ASC
))

CREATE TABLE [access_weights](
	[scenario] [tinyint] NULL,
	[increment] [int] NULL,
	[mgra] [smallint] NULL,
	[luz] [smallint] NULL,
	[weight] [float] NULL
)
GO
