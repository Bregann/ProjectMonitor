import type { NextApiRequest, NextApiResponse } from 'next'
import { DoGet } from '../../Helpers/apiHelper'
import { ActiveErrors, ErrorData } from '../../types/activeErrors'

export default async function handler(req: NextApiRequest, res: NextApiResponse<ActiveErrors>) {
    try {
        const apiRes = await DoGet('/api/GetData/GetActiveErrors');
        if(apiRes.status === 404){
            res.status(404).send({ error: "no data found", notFound: true});
            return;
        }

        const data: [] = await apiRes.json();
        res.status(200).json({errorData: data, notFound: false, error: ""});

    } catch (error) {
        res.status(500).json({ error: "error getting data", notFound: false});
    }
    

}