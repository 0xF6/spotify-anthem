export default async function <T>(url: string, token: string): Promise<T> {
    let request = new Request(url);
    request.headers.append("Authentication", token);
    let response = await fetch(request);
    if (response.ok)
        return await response.json() as T;
    else
        console.error(`Request failed: ${response.status}`);
    return (void 0) as unknown as T;
}