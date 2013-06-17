truncate table zum_hist_2004_ep;

insert into zum_hist_2004_ep
select x.zum,sum(e1.civ),sum(e2.civ),sum(p1.hs_sf),sum(p2.hs_sf),
sum(p1.hs_mf),sum(p2.hs_mf),sum(p1.hh),sum(p2.hh)

from concep_sr11.dbo.emp_2004_sgra_tab e2, concep_sr11.dbo.emp_2000_sgra_tab e1,
concep_sr11.dbo.popest_2000_sgra p1, concep_sr11.dbo.popest_2004_sgra p2,
xref_sgra_sr11 x

where x.sgra = e1.sgra and x.sgra = e2.sgra and x.sgra = p1.sgra and x.sgra = p2.sgra
group by x.zum;
