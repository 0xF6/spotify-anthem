export default async function <T>(url: string): Promise<T> {
    let response = await fetch(url);
    if (response.ok)
        return await response.json() as T;
    else
        console.error(`Request failed: ${response.status}`);
    return (void 0) as unknown as T;
}