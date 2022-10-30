export const DoGet = async (apiPath: string) => {
    return await fetch(process.env.NEXT_PUBLIC_WEB_URL + apiPath, { cache: 'no-store' });
}