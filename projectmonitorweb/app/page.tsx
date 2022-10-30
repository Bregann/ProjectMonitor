import React from "react";
import Dashboard from "./Dashboard";
import { DoGet } from "../Helpers/fetchHelper";
import { DashboardData } from "../types/dashboardData";
import { ActiveErrors } from "../types/activeErrors";

async function getDashboardData(){
    const res = await DoGet('/api/GetData');
    const data: DashboardData = await res.json();
    return data;
}

async function getErrorData(){
    const res = await DoGet('/api/GetActiveErrors');
    const data: ActiveErrors = await res.json();
    return data;
}

const Page = async() => {
    const dashData = await getDashboardData();
    const errorData = await getErrorData();

    return ( <Dashboard dashboardData={dashData} errorData={errorData}/>);
}
 
export default Page;
