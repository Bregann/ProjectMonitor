// Next.js API route support: https://nextjs.org/docs/api-routes/introduction
import type { NextApiRequest, NextApiResponse } from 'next'
import { resolve } from 'path';
import { DoGet } from '../../Helpers/apiHelper'
import { DashboardData } from '../../types/dashboardData'

export default async function handler( req: NextApiRequest, res: NextApiResponse<DashboardData>) {
  try {
    const apiRes = await DoGet('/api/GetData/GetDashboardData');
    res.status(200).json(await apiRes.json());
  } catch (error) {
      res.status(500).json({ error: "error getting data"});
  }
}
