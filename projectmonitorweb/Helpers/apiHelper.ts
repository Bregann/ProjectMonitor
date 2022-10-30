export const DoGet = async (apiUrl: string) => {
    return await fetch(process.env.API_URL + apiUrl, { cache: 'no-store' });
}

export const DoPost = async (apiUrl: string, data: any) => {
    const res = await fetch(process.env.API_URL + apiUrl, {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(data)
    });

    return res.json();
}

export const DoDelete = async(apiUrl: string, data: any) => {

    const res = await fetch(process.env.API_URL + apiUrl, {
        method: 'DELETE',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(data)
    });

    return res;
}

