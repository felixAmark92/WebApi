export default interface VideoOptions {
  autoplay: boolean;
  controls: boolean;
  responsive: boolean;
  fluid: boolean;
  sources: [
    {
      src: string;
      type: string;
    }
  ];
}
